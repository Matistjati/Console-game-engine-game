using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Uncoal.Engine;
using Uncoal.Internal;
using static Uncoal.Internal.NativeMethods;

namespace Uncoal.Runner
{
	internal static class FrameRunner
	{
		static TimeSpan lastFrameIteration;
		static Stopwatch frameCounter = new Stopwatch();

		static bool run;

		public static void Pause()
		{
			run = false;
			frameCounter.Stop();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void DestroyGameObjects()
		{
			// The only way to do kill an objects is to kill all references to it
			while (destructionQueue.Count != 0)
			{
				// Removing components from update
				GameObject gameObject = destructionQueue.Dequeue();

				if (gameObject?.components is null)
					return;

				// Just killing references
				foreach (Component component in gameObject?.components)
				{
					if (component is SpriteDisplayer sprite)
					{
						spritesToRemove.Enqueue(sprite);
					}

					component.gameObject = null;
					component.physicalState = null;
				}
				gameObject.physicalState = null;
				gameObject.components.Clear();
				gameObject.components = null;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void RemoveGameObjectsFromUpdate()
		{
			Delegate[] methods = updateCallBack?.GetInvocationList();

			Parallel.For(0, updateRemovalQueue.Count, (int j) =>
			{
				GameObject gameObject = updateRemovalQueue.Dequeue();

				// Avoiding nullreference
				if (gameObject?.components is null)
					// Parallel equivalent of break
					return;

				// Checking where they intersect and removing if so
				for (int i = 0; i < methods.Length; i++)
				{
					if (gameObject.components.Contains(methods[i].Target))
					{
						updateCallBack -= (Action)methods[i];
					}
				}
			});
		}


		public static void Run()
		{
			playField = new CHAR_INFO[Console.BufferWidth, Console.BufferHeight];

			lastFrameIteration = new TimeSpan();

			run = true;

			frameCounter.Start();

			Task renderSprites = Task.Run((Action)RenderSprites);

			while (run)
			{
				// Time Calculations

				TimeSpan elapsed = frameCounter.Elapsed;
				GameObject._timeDelta = (float)(elapsed - lastFrameIteration).TotalSeconds;

				GameObject._time = (float)elapsed.TotalSeconds;

				lastFrameIteration = elapsed;

				// Updating the public input api
				Input.UpdateInput();

				// Calling update
				updateCallBack?.Invoke();

				// When gameobjects actually get scheduled for destruction is reliant on the if statement down there
				// This is just to ensure that an object doesn't recognize that it should destroy itself more than one time in update
				if (updateRemovalQueue.Count != 0)
				{
					RemoveGameObjectsFromUpdate();
				}

				if (renderSprites.IsCompleted)
				{
					// Achieving thread safety by only modifying the gameobjects when no code using them is running
					if (destructionQueue.Count != 0)
					{
						DestroyGameObjects();
					}

					renderSprites = Task.Run((Action)RenderSprites);

					////// Debugging

					//renderSprites = Task.Run(() =>
					//{
					//	try
					//	{
					//		RenderSprites();
					//	}
					//	catch (Exception err)
					//	{
					//		Log.DefaultLogger.LogError(err);

					//		throw;
					//	}
					//});
				}
			}
		}

		// I estimate that maybe 40 gameobjects will exist at once?
		static List<SmallRectangle> spritePositions = new List<SmallRectangle>(40);

		static CHAR_INFO[,] playField;

		// Believe me, it can become about this big

		static Task writeConsole;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void RenderSprites()
		{
			while (spritesToReassign.Count != 0)
			{
				if (spritesToReassign.TryDequeue(out SpritePair spritePair))
				{
					spritePair.sprite.spriteMap = spritePair.newSprite;
				}
			}

			while (spritesToAdd.Count != 0)
			{
				if (spritesToAdd.TryDequeue(out SpriteDisplayer sprite))
				{
					RenderedGameObjects.Add(sprite);
				}
			}


			while (spritesToRemove.Count != 0)
			{
				if (spritesToRemove.TryDequeue(out SpriteDisplayer sprite))
				{
					RenderedGameObjects.Remove(sprite);
				}
			}


			// Clearing the internal color array representing the console
			ClearOldSprites();


			// Used for clearing the last frame drawing
			List<SmallRectangle> spritePositionsCopy = new List<SmallRectangle>(spritePositions);

			spritePositions.Clear();


			// Sort the render order based on layer ascending
			RenderedGameObjects.Sort((x, y) => y.Layer.CompareTo(x.Layer));

			// Filling colors and preparing spritepositions
			List<Task> fillColors = PrepareSpritePositionsAndFillColors();



			// Preparing what is used to build the new allrows
			PrepareLineInfo(spritePositionsCopy);

			// We have to finish filling colors before using it
			Task.WaitAll(fillColors.ToArray());

			// We don't want to risk changing allrows while still writing
			writeConsole?.Wait();

			// Joining allrows from colors

			// Handle to console output buffer
			IntPtr stdOutHandle = GetStdHandle(StdHandle.OutputHandle);

			// Writing all the rows to the console
			writeConsole = Task.Run(() =>
			{
				bool success = WriteConsoleW(
				stdOutHandle,   // The handle
				allRows,        // Characters to write
				allRows.Length, // Amount of characters to write
				out int charsWritten,
				IntPtr.Zero);// Reserved

				if (!success)
				{
					Log.DefaultLogger.LogError($"Error callong writeconsole: {Marshal.GetLastWin32Error()}");
				}
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static List<Task> PrepareSpritePositionsAndFillColors()
		{
			int colorWidth = playField.GetLength(0);
			int colorHeight = playField.GetLength(1);

			List<Task> fillColors = new List<Task>();

			for (int i = 0; i < RenderedGameObjects.Count; i++)
			{
				SpriteDisplayer sprite = RenderedGameObjects[i];

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

				// >> 1 is the same as division by 2
				position.X -= colorMapSizeX >> 1;

				position.Y -= colorMapSizeY >> 1;


				// Storing the position and dimensions of the sprite for later use
				spritePositions.Add(new SmallRectangle(
					(short)position.X,
					(short)position.Y,
					(short)colorMapSizeX,
					(short)colorMapSizeY));

				fillColors.Add(Task.Run(
					() => FillColors(position.X, position.Y, colorMapSizeX, colorMapSizeY, sprite)));
			}

			return fillColors;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void FillColors(int X, int Y, int colorMapSizeX, int colorMapSizeY, SpriteDisplayer sprite)
		{
			// Copying the rectangle

			if (X < 0)
			{
				colorMapSizeX += X;
				X = 0;
			}


			if (Y < 0)
			{
				colorMapSizeY += Y;
				Y = 0;
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
					CHAR_INFO cell = sprite.Sprite.spriteMap[x, y];


					if (cell[0] == ' ')
						if (playField[x + X, y + Y] != null)
							continue;

					playField[x + X, y + Y] = cell;
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


					int totalHeight = positivePositionY + spritePositions[i].Height;

					int totalWidth = positivePositionX + spritePositions[i].Width;

					Parallel.For(positivePositionX, totalWidth,
						x =>
						{
							for (int y = positivePositionY; y < totalHeight; y++)
							{
								playField[x, y] = null;
							}
						});
				});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void PrepareLineInfo(List<SmallRectangle> spritePositionsCopy)
		{
			filledRows.Clear();

			/////////////////////////////
			// Hashing the y positions //
			/////////////////////////////

			int colorWidth = playField.GetLength(0);
			int colorHeight = playField.GetLength(1);

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

			Task.WaitAll(iterateSpritePos, iterateSpritePosCopy);
		}


		public static Queue<GameObject> destructionQueue = new Queue<GameObject>();
		public static Queue<GameObject> updateRemovalQueue = new Queue<GameObject>();

		public static List<SpriteDisplayer> RenderedGameObjects = new List<SpriteDisplayer>();
		public static ConcurrentQueue<SpriteDisplayer> spritesToRemove = new ConcurrentQueue<SpriteDisplayer>();
		public static ConcurrentQueue<SpriteDisplayer> spritesToAdd = new ConcurrentQueue<SpriteDisplayer>();
		public static ConcurrentQueue<SpritePair> spritesToReassign = new ConcurrentQueue<SpritePair>();

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
