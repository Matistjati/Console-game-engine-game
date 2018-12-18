using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Uncoal.Engine;
using static Uncoal.Internal.NativeMethods;
using Uncoal.Internal;

namespace Uncoal.Runner
{
	internal static class FrameRunner
	{
		static TimeSpan lastFrameCall;
		static Stopwatch frameMeasurer = new Stopwatch();

		static bool run;

		public static void Pause()
		{
			run = false;
			frameMeasurer.Stop();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void DestroyGameObjects()
		{
			// The only way to do kill an objects is to kill all references to it
			Delegate[] methods = updateCallBack?.GetInvocationList();
			while (destructionQueue.Count != 0)
			{
				// Removing components from update
				GameObject gameObject = destructionQueue.Dequeue();
				List<Delegate> componentsToRemove = new List<Delegate>();

				if (gameObject.components is null)
					continue;


				for (int i = 0; i < methods.Length; i++)
				{
					if (gameObject.components.Contains(methods[i].Target))
					{
						updateCallBack -= (Action)methods[i];
					}
				}

				// Just killing references
				foreach (Component component in gameObject?.components)
				{
					if (component is SpriteDisplayer sprite)
					{
						RenderedGameObjects.Remove(sprite);
					}

					component.gameObject = null;
					component.physicalState = null;
				}
				gameObject.physicalState = null;
				gameObject.components.Clear();
				gameObject.components = null;
			}
		}



		public static void Run()
		{
			lastFrameCall = new TimeSpan();

			run = true;

			frameMeasurer.Start();

			Task renderSprites = Task.Run((Action)RenderSprites);


			while (run)
			{
				// Time Calculations

				TimeSpan elapsed = frameMeasurer.Elapsed;
				GameObject._timeDelta = (float)(elapsed - lastFrameCall).TotalSeconds;

				GameObject._time = (float)elapsed.TotalSeconds;

				lastFrameCall = elapsed;

				// Updating the public input api
				Input.UpdateInput();

				// Calling update
				updateCallBack?.Invoke();

				//RenderSprites();

				// Rendering has is done on another thread
				//if (renderSprites.Exception != null)
				//{	}

				if (renderSprites.IsCompleted)
				{
					renderSprites = Task.Run((Action)RenderSprites);
				}
			}
		}

		const char whiteSpaceChar = ' ';
		const string whiteSpaceString = " ";
		const string escapeStartNormal = "\x1b[";

		// I estimate that maybe 40 gameobjects will exist at once?
		static List<SmallRectangle> spritePositions = new List<SmallRectangle>(40);
		static List<SmallRectangle> spritePositionsCopy = new List<SmallRectangle>(40);
		static Dictionary<ushort, Distance> filledRows = new Dictionary<ushort, Distance>(40);

		static string[,] colors = new string[Console.BufferWidth, Console.BufferHeight];

		// Believe me, it can become about this big
		static StringBuilder allRows = new StringBuilder(564000);

		static Task writeConsole;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void RenderSprites()
		{
			Task reassignSprites = Task.Run(() =>
			{
				while (spritesToReassign.Count != 0)
				{
					SpritePair spritePair = spritesToReassign.Dequeue();
					spritePair.sprite.colorValues = spritePair.newSprite;
				}
			});

			// Clearing the internal color array representing the console
			ClearOldSprites();

			// Sort the render order based on layer ascending
			RenderedGameObjects.Sort((x, y) => y.Layer.CompareTo(x.Layer));


			// Used in for clearing the old frame drawing
			spritePositionsCopy = new List<SmallRectangle>(spritePositions);

			spritePositions.Clear();

			reassignSprites.Wait();

			//Log.DefaultLogger.LogInfo("filling colors");
			FillColors();


			// Preparing what is used to build the new allrows
			PrepareLineInfo();


			// We don't want to risk changing allrows while still writing
			writeConsole?.Wait();

			// Joining allrows from colors
			JoinColorsToString();

			// Handle to console output buffer
			IntPtr stdOutHandle = GetStdHandle(StdHandle.OutputHandle);

			// Writing all the rows to the console
			writeConsole = Task.Run(() =>
			WriteConsoleW(
				stdOutHandle,   // The handle
				allRows,        // Characters to write
				allRows.Length, // Amount of characters to write
				out int charsWritten,
				IntPtr.Zero));  // Reserved

			// Destroying all gameobjects in destructionQueue
			if (destructionQueue.Count != 0)
			{
				DestroyGameObjects();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void FillColors()
		{
			int colorWidth = colors.GetLength(0);
			int colorHeight = colors.GetLength(1);

			List<SpriteDisplayer> localGameObjects = new List<SpriteDisplayer>(RenderedGameObjects);
			for (int i = 0; i < localGameObjects.Count; i++)
			{
				SpriteDisplayer sprite = localGameObjects[i];

				// Only render the visible ones
				if (!sprite.IsVisible)
					continue;

				// Caching the position of the object to be rendered 
				Coord position = (Coord)sprite.physicalState.Position;

				// Checking that the sprite is on-screen
				if (position.X > colorWidth || position.Y > colorHeight)
					continue;

				// Caching the size of the sprite
				int colorMapSizeX = sprite.ColorMap.GetLength(0);
				int colorMapSizeY = sprite.ColorMap.GetLength(1);

				// Assuring we won't go out of bounds
				if (colorMapSizeX + position.X > colorWidth)
				{
					// This is basically a shortened version of position.X - (position.X + colorMapSize.X - displaySize.X)
					colorMapSizeX = (colorWidth - position.X);

					// The above formula sets colormapsize.x equal to  the max width, problem is arrays start at 0
					// We do this check to ensure that colorMapSize.X does not go below 0
					if (colorMapSizeX != 0)
						colorMapSizeX--;
				}


				if (colorMapSizeY + position.Y > colorHeight)
				{
					// This is basically a shortened version of position.Y - (position.Y + colorMapSize.Y - displaySize.Y)
					colorMapSizeY = (colorHeight - position.Y);

					if (colorMapSizeY != 0)
						// The above formula sets colormapsize.Y equal to  the max width, problem is arrays start at 0
						// We do this check to ensure that colorMapSize.Y does not go below 0
						colorMapSizeY--;
				}

				// Same  as division by 2
				int xOffset = colorMapSizeX >> 1;
				int yOffset = colorMapSizeY >> 1;

				position.X -= xOffset;

				position.Y -= yOffset;


				// Storing the position and dimensions of the sprite for later use
				spritePositions.Insert(0, new SmallRectangle(
					(short)position.X,
					(short)position.Y,
					(short)colorMapSizeX,
					(short)colorMapSizeY));



				if (position.X < 0)
				{
					colorMapSizeX += position.X;
					position.X = 0;
				}


				if (position.Y < 0)
				{
					colorMapSizeY += position.Y;
					position.Y = 0;
				}

			

				if (sprite.ColorMap.GetLength(0) <= colorMapSizeX)
				{
					colorMapSizeX = sprite.ColorMap.GetLength(0);
				}

				if (sprite.ColorMap.GetLength(1) <= colorMapSizeY)
				{
					colorMapSizeY = sprite.ColorMap.GetLength(1);
				}
				// Filling our internal array (strings representing colors) representing the console


				for (int x = 0; x < colorMapSizeX; x++)
				{
					for (int y = 0; y < colorMapSizeY; y++)
					{
						string cellColor = sprite.ColorMap[x, y];


						if (cellColor[0] == ' ')
							if (colors[x + position.X, y + position.Y] != null)
								continue;

						colors[x + position.X, y + position.Y] = cellColor;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void ClearOldSprites()
		{
			Parallel.For(0, spritePositions.Count,
				i =>
				{
					// Getting a positive version of spritepositions x and y
					int positivePositionX = (spritePositions[i].X < 0)
						? 0
						: spritePositions[i].X;

					int positivePositionY = (spritePositions[i].Y < 0)
						? 0
						: spritePositions[i].Y;

					Parallel.For(0, spritePositions[i].Height,
						y =>
						{
							for (int x = 0; x < spritePositions[i].Width; x++)
							{
								colors[positivePositionX + x, positivePositionY + y] = null;
							}
						});
				});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void PrepareLineInfo()
		{
			filledRows.Clear();

			/////////////////////////////
			// Hashing the y positions //
			/////////////////////////////

			int colorWidth = colors.GetLength(0);
			int colorHeight = colors.GetLength(1);

			// Filling with sprites to rendered
			Task iterateSpritePos = Task.Run(() =>
			{
				for (int i = 0; i < spritePositions.Count; i++)
				{
					// Combining concepts of for and foreach for even faster
					SmallRectangle rectangle = spritePositions[i];

					if (rectangle.Y < 0)
					{
						rectangle.Height += rectangle.Y;
						rectangle.Y = 0;
					}


					for (ushort y = (ushort)rectangle.Y; y < rectangle.Height + rectangle.Y; y++)
					{
						if (filledRows.TryGetValue(y, out Distance currentRow))
						{
							if (rectangle.X < currentRow.start && rectangle.X > 0)
							{
								currentRow.length += currentRow.start - rectangle.X;

								currentRow.start = rectangle.X;
							}
							else
							{
								currentRow.length += rectangle.X - currentRow.start;

								if (currentRow.length + currentRow.start > colorWidth)
								{
									currentRow.length = colorWidth - currentRow.start;
								}
							}

							// Enable if we want to use sprites of different sizes
							if (currentRow.length < rectangle.Width)
							{
								currentRow.length = rectangle.Width;
							}

							filledRows[y] = currentRow;
						}
						else
						{
							filledRows[y] = new Distance(
								Start: (rectangle.X < 0) ? 0 : rectangle.X,
								Length: rectangle.Width);
						}
					}
				}
			});

			// Sprites to be removed
			Task iterateSpritePosCopy = Task.Run(() =>
			{
				for (int i = 0; i < spritePositionsCopy.Count; i++)
				{
					// Combining concepts of for and foreach for even faster
					SmallRectangle rectangle = spritePositionsCopy[i];

					if (rectangle.Y < 0)
					{
						rectangle.Height += rectangle.Y;
						rectangle.Y = 0;
					}


					for (ushort y = (ushort)rectangle.Y; y < rectangle.Height + rectangle.Y; y++)
					{
						if (filledRows.TryGetValue(y, out Distance currentRow))
						{
							if (rectangle.X < currentRow.start && rectangle.X > 0)
							{
								currentRow.length += currentRow.start - rectangle.X;

								currentRow.start = rectangle.X;
							}
							else
							{
								currentRow.length += rectangle.X - currentRow.start;

								if (currentRow.length + currentRow.start > colorWidth)
								{
									currentRow.length = colorWidth - currentRow.start;
								}
							}

							// Enable if we want to use sprites of different sizes
							if (currentRow.length < rectangle.Width)
							{
								currentRow.length = rectangle.Width;
							}

							filledRows[y] = currentRow;
						}
						else
						{
							filledRows[y] = new Distance(
								Start: (rectangle.X < 0) ? 0 : rectangle.X,
								Length: rectangle.Width);
						}
					}
				}
			});

			Task clearOldSprites = Task.Run(() =>
			{
				for (int i = 0; i < spritePositionsCopy.Count; i++)
				{
					SmallRectangle rect = spritePositionsCopy[i];

					int width = rect.Width + rect.X;
					int height = rect.Height + rect.Y;

					for (short x = rect.X; x < width; x++)
					{
						for (short y = rect.Y; y < height; y++)
						{
							if (colors[x, y] is null)
							{
								colors[x, y] = whiteSpaceString;
							}
						}
					}
				}
			});

			Task.WaitAll(iterateSpritePos, iterateSpritePosCopy, clearOldSprites);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void JoinColorsToString()
		{
			allRows.Clear();

			int colorHeight = colors.GetLength(1);

			////////////////////////////////////////////////////////////
			// Using the y positions to join everything into a string //
			////////////////////////////////////////////////////////////

			for (ushort y = 0; y < colorHeight; y++)
			{
				// Checking if there is an object on this row
				// Otherwise, we sipmly append a newline
				if (filledRows.TryGetValue(y, out Distance rowInfo))
				{
					// Spacing from left to the start of the sprite
					// Uses an escape sequence
					// https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences#cursor-positioning

					allRows.Append(escapeStartNormal);
					allRows.Append(rowInfo.start + "C");

					for (int x = 0; x < rowInfo.length; x++)
					{
						// An escape sequence telling the console what color to display
						// For more info, check
						// https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences#extended-colors


						allRows.Append(colors[x + rowInfo.start, y] ?? whiteSpaceString);
					}
					allRows.Append("\n");
				}
				else
				{
					allRows.Append("\n");
				}
			}
		}

		public static Queue<SpritePair> spritesToReassign = new Queue<SpritePair>();
		public static Queue<GameObject> destructionQueue = new Queue<GameObject>();
		public static List<SpriteDisplayer> RenderedGameObjects = new List<SpriteDisplayer>();

		static void DoNothing() { }
		static Action updateCallBack = new Action(DoNothing);

		public static void AddFrameSubscriber(Action method)
		{
			updateCallBack += method;
		}

		public static void Unsubscribe(Action method)
		{
			updateCallBack -= method;
		}

		public static void UnsubscribeAll()
		{
			updateCallBack = new Action(DoNothing);
			updateCallBack -= DoNothing;
		}
	}
}
