﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
			Delegate[] methods = updateCallBack.GetInvocationList();
			while (destructionQueue.Count != 0)
			{
				// Removing components from update
				GameObject gameObject = destructionQueue.Dequeue();
				List<Delegate> componentsToRemove = new List<Delegate>();

				for (int i = 0; i < methods.Length; i++)
				{
					if (gameObject.components.Contains(methods[i].Target))
					{
						updateCallBack -= (Action)methods[i];
					}
				}

				// Just killing references
				foreach (Component component in gameObject.components)
				{
					if (component.GetType() == typeof(SpriteDisplayer))
					{
						RenderedGameObjects.Remove((SpriteDisplayer)component);
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
			StringBuilder sprite = new StringBuilder(colors.GetLength(0) * colors.GetLength(1));
			for (int y = 0; y < colors.GetLength(1); y++)
			{
				for (int x = 0; x < colors.GetLength(0); x++)
				{
					sprite.Append(colors[x, y]);
				}
				sprite.Append('\n');
			}
			Console.Write(sprite);
		}
#endif

		internal static int framesBetweenDraws = 2;

		private static float total;
		private static void TestTimeAccuracy()
		{
			total += GameObject.TimeDelta;
			Console.Clear();
			Console.Write($"time: {GameObject.Time}\n" +
				$"timedelta total: {total}\n" +
				$"timedelta: {GameObject.TimeDelta}\n" +
				$"difference: {total - GameObject.Time}");
		}

		public static void Run()
		{
			lastFrameCall = new TimeSpan();

			displaySize = new Coord((uint)Console.BufferWidth, (uint)Console.BufferHeight);

			colors = new string[displaySize.X, displaySize.Y];

			allRows = new StringBuilder(RenderedGameObjects.Count * 32 * 32);

			run = true;

			frameMeasurer.Start();
			int framesSinceLastDraw = 0;
			//updateCallBack += TestTimeAccuracy;
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

				if (framesSinceLastDraw >= framesBetweenDraws)
				{
					RenderSprites();
					framesSinceLastDraw = 0;
				}
				else
				{
					framesSinceLastDraw++;
				}

				// Destroying all gameobjects in destructionQueue
				DestroyGameObjects();

				// This is to avoid the console flickering randomly
				// Grant developer ability to change this value
				//Thread.Sleep(10);
			}
		}

		const char whiteSpace = ' ';
		const char colorSeparator = ';';
		const string escapeStartRGB = "\x1b[38;2;";
		const string escapeStartNormal = "\x1b[";
		const string escapeEnd = "m█";

		static List<SmallRectangle> spritePositions = new List<SmallRectangle>();
		static List<SmallRectangle> spritePositionsCopy = new List<SmallRectangle>();

		static string[,] colors;
		static Coord displaySize;

		static StringBuilder allRows;

		static void RenderSprites()
		{
			// Used in clearconsole for clearing the old frame drawing
			// Copying spritepositions before clearing it is important
			spritePositionsCopy = new List<SmallRectangle>(spritePositions);

			spritePositions.Clear();

			// Sort the render order based on layer
			RenderedGameObjects.Sort((x, y) => x.Layer.CompareTo(y.Layer));

			for (int i = RenderedGameObjects.Count - 1; i >= 0; i--)
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
				Coord colorMapSize = new Coord(RenderedGameObjects[i].ColorMap.GetLength(0), RenderedGameObjects[i].ColorMap.GetLength(0));

				// Assuring we won't go out of bounds
				if (colorMapSize.X + position.X > displaySize.X)
					colorMapSize.SetX(displaySize.X - position.X);

				if (colorMapSize.Y + position.Y > displaySize.Y)
					colorMapSize.SetY(displaySize.Y - position.Y);

				// Storing the position and dimensions of the sprite for later use
				spritePositions.Insert(0, new SmallRectangle(
					(ushort)position.X,
					(ushort)position.Y,
					(ushort)colorMapSize.X,
					(ushort)colorMapSize.Y));

				// Filling our internal array (strings representing colors) representing the console
				for (int y = 0; y < colorMapSize.Y; y++)
				{
					for (int x = 0; x < colorMapSize.X; x++)
					{
						string cellColor = RenderedGameObjects[i].ColorMap[x, y];

						if (string.IsNullOrWhiteSpace(cellColor))
							if (!(colors[x + position.X, y + position.Y] is null))
								continue;


						colors[x + position.X, y + position.Y] = cellColor;
					}
				}
			}

			// Clearing the old rows
			allRows.Clear();

			//
			// Joining all the rows
			// 

			// Hashing the y positions
			Dictionary<ushort, SmallRectangle> yFilledRows = new Dictionary<ushort, SmallRectangle>(spritePositions.Count);
			for (int i = 0; i < spritePositions.Count; i++)
			{
				if (!yFilledRows.ContainsKey(spritePositions[i].Y))
				{
					yFilledRows.Add(spritePositions[i].Y, spritePositions[i]);
				}
			}

			// Caching the array size
			int colorWidth = colors.GetLength(0);
			int colorHeight = colors.GetLength(1);

			for (ushort y = 0; y < colorHeight; y++)
			{
				// Checking if there is an object on this row
				// Otherwise, we sipmly append a newline
				if (yFilledRows.TryGetValue(y, out SmallRectangle rowInfo))
				{
					// Here, we iterate through the sprite height and increment the outer y manually
					for (int spriteY = 0; spriteY < rowInfo.Height; spriteY++)
					{
						y++;

						// Spacing from left to the start of the sprite
						// Uses an escape sequence
						// https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences#cursor-positioning

						allRows.Append(escapeStartNormal);
						allRows.Append(rowInfo.X + "C");

						for (int x = 0; x < rowInfo.Width; x++)
						{
							// An escape sequence telling the console what color to display
							// For more info, check
							// https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences#extended-colors

							allRows.Append(colors[x + rowInfo.X, y]);
						}
						allRows.Append(Environment.NewLine);
					}
				}
				else
				{
					allRows.Append(Environment.NewLine);
				}
			}

			// Handle to console output buffer
			IntPtr stdOutHandle = GetStdHandle(StdHandle.OutputHandle);

			// Clearing the sprites from the last render
			ClearConsle(stdOutHandle);

			// Writing all the rows to the console
			WriteConsoleW(
				stdOutHandle,   // The handle
				allRows,        // Characters to write
				allRows.Length, // Amount of characters to write
				out int charsWritten,
				IntPtr.Zero);   // Reserved


			// Clearing the internal color array representing the console
			for (int i = 0; i < spritePositions.Count; i++)
			{
				for (int y = 0; y < spritePositions[i].Height; y++)
				{
					for (int x = 0; x < spritePositions[i].Width; x++)
					{
						colors[spritePositions[i].X + x, spritePositions[i].Y + y] = null;
					}
				}
			}
		}

		static void ClearConsle(IntPtr stdOut)
		{
			// Clearing the console Using some p/invoking
			for (int i = 0; i < spritePositionsCopy.Count; i++)
			{
				// Converting the position to coordinates
				COORD position = new COORD((short)spritePositionsCopy[i].X, (short)spritePositionsCopy[i].X);

				for (int y = 0; y < spritePositionsCopy[i].Height; y++)
				{
					FillConsoleOutputCharacter(
						stdOut,     // Output buffer handle
						whiteSpace, // The character we replace stuff with
						spritePositionsCopy[i].Width, // Amount of times to replace character
						position,   // The are to start writing
						out int lpNumberOfCharsWritten);

					position.Y++;
				}
			}
		}

		public static Queue<GameObject> destructionQueue = new Queue<GameObject>();
		public static List<SpriteDisplayer> RenderedGameObjects { get; set; } = new List<SpriteDisplayer>();

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
