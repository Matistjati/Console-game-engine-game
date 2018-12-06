using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using WMPRichPreviewLauncher;
using System.Text;
using System.Threading.Tasks;
using Uncoal.Engine;
using static Uncoal.Internal.NativeMethods;

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



#if DEBUG
		// For checking if the rendering or the internal representation of the sprite is at fault

		public static void ColorMapPrint()
		{
			Console.Clear();

			StringBuilder sprite = new StringBuilder(colors.GetLength(0) * colors.GetLength(1));
			int width = colors.GetLength(0);
			int height = colors.GetLength(1);
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					string color = colors[x, y];
					if (color is null)
					{
						sprite.Append(" ");
					}
					else
					{
						sprite.Append(color);
					}
				}
				sprite.Append('\n');
				Console.Write(sprite);
				sprite.Clear();
			}
		}

		public static bool IsColorsAllNull()
		{
			for (int y = 0; y < colors.GetLength(1); y++)
			{
				for (int x = 0; x < colors.GetLength(0); x++)
				{
					if (colors[x, y] != null)
					{
						return false;
					}
				}
			}
			return true;
		}

		private static float total;
		private static readonly DateTime startDate;
		private static void TestTimeAccuracy()
		{
			total += GameObject.TimeDelta;
			Console.Clear();
			Console.Write($"time: {GameObject.Time}\n" +
				$"timedelta total: {total}\n" +
				$"timedelta: {GameObject.TimeDelta}\n" +
				$"difference: {total - GameObject.Time}\n" +
				$"Actual time: {(DateTime.UtcNow - startDate).TotalSeconds}");
		}
#endif

		static Task renderSprites;
		static Action renderSpritesAction = (Action)RenderSprites;

		public static void Run()
		{
			lastFrameCall = new TimeSpan();

			displaySize = new Coord(Console.BufferWidth, Console.BufferHeight);

			colors = new string[displaySize.X, displaySize.Y];

			run = true;

			frameMeasurer.Start();


			renderSprites = Task.Run(renderSpritesAction);
			
			// Debug time calculation accuracy
			//updateCallBack += TestTimeAccuracy;
			//startDate = DateTime.UtcNow;


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

				// Note to self:
				// If two objects overlap, make sure that the bigger one has the lowest layer
				// Rendering at (0,0) causes problems, too lazy to fix tho..

				if (renderSprites.IsCompleted)
				{
					renderSprites = Task.Run(renderSpritesAction);
				}


				// Destroying all gameobjects in destructionQueue
				if (destructionQueue.Count != 0)
				{
					DestroyGameObjects();
				}
			}
		}

		const char whiteSpaceChar = ' ';
		const string escapeStartNormal = "\x1b[";

		// I estimate that maybe 40 gameobjects will exist at once?
		static List<SmallRectangle> spritePositions = new List<SmallRectangle>(40);
		static List<SmallRectangle> spritePositionsCopy = new List<SmallRectangle>(40);
		static Dictionary<ushort, Distance> filledRows = new Dictionary<ushort, Distance>(40);

		static string[,] colors;
		static Coord displaySize;

		// Believe me, it will be about this big
		static StringBuilder allRows = new StringBuilder(244000);

		static Task writeConsole;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void RenderSprites()
		{

			// Clearing the internal color array representing the console
			ClearOldSprites();

			// Used in clearconsole for clearing the old frame drawing
			// Copying spritepositions before clearing it is important
			spritePositionsCopy = new List<SmallRectangle>(spritePositions);

			spritePositions.Clear();

			// Sort the render order based on layer descending when the size changes

			RenderedGameObjects.Sort((x, y) => y.Layer.CompareTo(x.Layer));

			// Filling colors based on the current RenderedGameObjects
			FillColors();

			// We don't want to risk changing allrows while still writing
			writeConsole?.Wait();


			// Joining allrows from colors
			JoinColorsToString();

			// Handle to console output buffer
			IntPtr stdOutHandle = GetStdHandle(StdHandle.OutputHandle);

			// Clearing the sprites from the last render
			ClearConsole(stdOutHandle);

			// Writing all the rows to the console
			writeConsole = Task.Run(() =>
			WriteConsoleW(
				stdOutHandle,   // The handle
				allRows,        // Characters to write
				allRows.Length, // Amount of characters to write
				out int charsWritten,
				IntPtr.Zero));  // Reserved
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void FillColors()
		{
			// Iterate the lowest priority layers first, as they are later overwritten
			for (int i = 0; i < RenderedGameObjects.Count; i++)
			{
				// Only render the visible ones
				if (!RenderedGameObjects[i].IsVisible)
					continue;

				// Caching the position of the object to be rendered 
				Coord position = (Coord)RenderedGameObjects[i].physicalState.Position;

				// Checking that the sprite is on-screen
				if (position.X > displaySize.X || position.Y > displaySize.Y)
					continue;

				// Caching the size of the sprite
				Coord colorMapSize = new Coord(
					RenderedGameObjects[i].ColorMap.GetLength(0),
					RenderedGameObjects[i].ColorMap.GetLength(1));

				// Assuring we won't go out of bounds
				if (colorMapSize.X + position.X > displaySize.X)
				{
					// This is basically a shortened version of position.X - (position.X + colorMapSize.X - displaySize.X)
					colorMapSize.X = (displaySize.X - position.X);

					// The above formula sets colormapsize.x equal to  the max width, problem is arrays start at 0
					// We do this check to ensure that colorMapSize.X does not go below 0
					if (colorMapSize.X != 0)
						colorMapSize.X--;
				}


				if (colorMapSize.Y + position.Y > displaySize.Y)
				{
					// This is basically a shortened version of position.Y - (position.Y + colorMapSize.Y - displaySize.Y)
					colorMapSize.Y = (displaySize.Y - position.Y);

					if (colorMapSize.X != 0)
						// The above formula sets colormapsize.Y equal to  the max width, problem is arrays start at 0
						// We do this check to ensure that colorMapSize.Y does not go below 0
						colorMapSize.X--;
				}

				int xOffset = colorMapSize.X / 2;
				int yOffset = colorMapSize.Y / 2;

				position.X -= xOffset;

				position.Y -= yOffset;


				// Storing the position and dimensions of the sprite for later use
				spritePositions.Insert(0, new SmallRectangle(
					(short)position.X,
					(short)position.Y,
					(short)colorMapSize.X,
					(short)colorMapSize.Y));

				// What we will use for indexing the color array

				// What we will use for indexing the color array
				int xIndex;
				int yIndex;

				// Filling our internal array (strings representing colors) representing the console

				// X and Y are for the array of the gameobject, which are then added to (xIndex and yIndex) to get the index of colors

				for (int x = 0; x < colorMapSize.X; x++)
				{
					if ((xIndex = x + position.X) < 0)
						continue;

					for (int y = 0; y < colorMapSize.Y; y++)
					{
						string cellColor = RenderedGameObjects[i].ColorMap[x, y];

						if ((yIndex = y + position.Y) < 0)
							continue;

						// cellcolor will only have length 1 if it is whitespace (due to the way cellcolor is assigned)
						// This can be found in sprite.cs under ctor: public Sprite(Bitmap image, float scale)

						if (cellColor.Length == 1) // cellColor == " " but faster
							if (colors[xIndex, yIndex] != null)
								continue;

						colors[xIndex, yIndex] = cellColor;
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
		static void JoinColorsToString()
		{
			allRows.Clear();
			filledRows.Clear();

			/////////////////////////////
			// Hashing the y positions //
			/////////////////////////////

			for (int i = 0; i < spritePositions.Count; i++)
			{
				for (short y = 0; y < spritePositions[i].Height; y++)
				{
					short index = (short)(spritePositions[i].Y + y);
					// spritePositions[i].Y + y (spritePositions[i].Y might be negative)
					if (index < 0)
						continue;

					if (filledRows.TryGetValue((ushort)index, out Distance currentRow))
					{
						if (spritePositions[i].X < currentRow.start && spritePositions[i].X > 0)
						{
							currentRow.length += currentRow.start - spritePositions[i].X;

							currentRow.start = spritePositions[i].X;
						}
						else
						{
							currentRow.length += spritePositions[i].X - currentRow.start;

							if (currentRow.length + currentRow.start > displaySize.X)
							{
								currentRow.length = displaySize.X - currentRow.start;
							}
						}

						// Enable if we want to use sprites of different sizes
						if (spritePositions[i].Width > currentRow.length)
						{
							currentRow.length = spritePositions[i].Width;
						}

						filledRows[(ushort)index] = currentRow;
					}
					else
					{
						int positiveX = (spritePositions[i].X < 0)
							? 0
							: spritePositions[i].X;

						filledRows[(ushort)index] = new Distance(positiveX, spritePositions[i].Width);
					}
				}
			}

			////////////////////////////////////////////////////////////
			// Using the y positions to join everything into a string //
			////////////////////////////////////////////////////////////

			//Caching the array size
			int colorHeight = colors.GetLength(1);

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

						allRows.Append(colors[x + rowInfo.start, y] ?? " ");
					}
					allRows.Append(Environment.NewLine);
				}
				else
				{
					allRows.Append(Environment.NewLine);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void ClearConsole(IntPtr stdOut)
		{
			// Clearing the console Using some p/invoking
			Parallel.For(0, spritePositionsCopy.Count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
				(int index) =>
				{
					COORD position = new COORD(spritePositionsCopy[index].X, spritePositionsCopy[index].X);

					Parallel.For(0, spritePositionsCopy[index].Height, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
					indexInner =>
					   {
						   COORD pos = position;
						   pos.Y += (short)indexInner;

						   FillConsoleOutputCharacter(
								stdOut,     // Output buffer handle
								whiteSpaceChar, // The character we replace stuff with
								spritePositionsCopy[index].Width, // Amount of times to replace character
								pos,   // The position to start writing
								out int lpNumberOfCharsWritten);
					   });
				});
		}

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
