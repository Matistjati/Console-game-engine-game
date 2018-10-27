using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using static Console_game.NativeMethods;

namespace Console_game
{
	internal static class FrameRunner
	{
		static DateTime lastFrameCall;
		static DateTime start;

		static bool run;

		static bool firstRun = true;

		static DateTime pauseStartTime;
		public static void Pause()
		{
			run = false;
			pauseStartTime = DateTime.Now;
		}

		internal static void DestroyGameObjects()
		{
			// The only way to do kill an objects is to kill all references to it
			for (int i = 0; i < destructionQueue.Count; i++)
			{
				GameObject gameObject = destructionQueue.Dequeue();
				List<Component> componentsToRemove = new List<Component>();
				var a = updateCallBack.GetMethodInfo();
				/*foreach (Component componentToDestroy in updateCallBack.Where(component => component.gameObject == gameObject))
				{
					componentsToRemove.Add(componentToDestroy);
				}
				for (int o = 0; o < componentsToRemove.Count; o++)
				{
					updateCallBack.Remove(componentsToRemove[i]);
				}*/

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

		const char whiteSpace = ' ';
		const char colorSeparator = ';';
		const string escapeStart = "\x1b[38;2;";
		const string escapeStar2t = "\x1b[";
		const string escapeEnd = "m█";
		static RGB[,] colors;
		static List<SmallRectangle> spritePositions = new List<SmallRectangle>();
		static Coord displaySize;
		static readonly RGB emptyColor = new RGB(0, 0, 0);

		static StringBuilder allRows;

#if DEBUG
		public static void ColorMapPrint()
		{
			StringBuilder sprite = new StringBuilder(colors.GetLength(0) * colors.GetLength(1));
			for (int y = 0; y < colors.GetLength(1); y++)
			{
				for (int x = 0; x < colors.GetLength(0); x++)
				{
					RGB rgb = colors[x, y];
					if (rgb is null)
					{
						sprite.Append(" ");
					}
					else
					{
						sprite.Append($"\x1b[38;2;{rgb.R};{rgb.G};{rgb.B}m█");
					}
				}
				sprite.Append('\n');
			}
			Console.Write(sprite);
		}
#endif

		public static void Run()
		{
			if (firstRun)
			{
				start = DateTime.Now;
				firstRun = false;
			}
			else
			{
				start += DateTime.Now - pauseStartTime;
			}

			lastFrameCall = DateTime.Now;

			displaySize = new Coord((uint)Console.BufferWidth, (uint)Console.BufferHeight);

			colors = new RGB[displaySize.X, displaySize.Y];

			allRows = new StringBuilder(RenderedGameObjects.Count * 32 * 32);

			run = true;
			while (run)
			{
				// Time Calculations
				GameObject._timeDelta = (float)(DateTime.Now - lastFrameCall).TotalSeconds;

				GameObject._time = (float)(DateTime.Now - start).TotalSeconds;

				lastFrameCall = DateTime.Now;

				// Updating the public input api
				Input.UpdateInput();

				// Calling update
				updateCallBack?.Invoke();

				// Note to self:
				// If two objects overlap, make sure that the bigger one has the lowest layer
				// Rendering at (0,0) causes problems, too lazy to fix tho..

				List<SmallRectangle> spritePositionsCopy = new List<SmallRectangle>(spritePositions);

				spritePositions = new List<SmallRectangle>();

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
						colorMapSize.SetX(colorMapSize.X - (colorMapSize.X + position.X - displaySize.X) - 1);

					if (colorMapSize.Y + position.Y > displaySize.Y)
						colorMapSize.SetY(colorMapSize.Y - (colorMapSize.Y + position.Y - displaySize.Y) - 1);

					spritePositions.Insert(0, new SmallRectangle(
						(ushort)position.X,
						(ushort)position.Y,
						(ushort)colorMapSize.X,
						(ushort)colorMapSize.Y));

					for (int y = 0; y < colorMapSize.Y; y++)
					{
						for (int x = 0; x < colorMapSize.X; x++)
						{
							RGB rgb = RenderedGameObjects[i].ColorMap[x, y];
							if (rgb.isEmpty)
								continue;

							colors[x + position.X, y + position.Y] = rgb;
						}
					}
				}

				IntPtr stdOutHandle = GetStdHandle(StdHandle.OutputHandle);


				// Joining all the rows
				// TODO proper distancing between sprites
				allRows.Clear();

				// Hashing the y positions
				Dictionary<ushort, SmallRectangle> yFilledRows = new Dictionary<ushort, SmallRectangle>(spritePositions.Count);
				for (int i = 0; i < spritePositions.Count; i++)
				{
					if (!yFilledRows.ContainsKey(spritePositions[i].Y))
					{
						yFilledRows.Add(spritePositions[i].Y, spritePositions[i]);
					}
				}


				int colorWidth = colors.GetLength(0);
				int colorHeight = colors.GetLength(1);
				for (ushort y = 0; y < colorHeight; y++)
				{
					if (yFilledRows.TryGetValue(y, out SmallRectangle rowInfo))
					{
						for (int spriteY = 0; spriteY < rowInfo.Height; spriteY++)
						{
							y++;
							allRows.Append(escapeStar2t);
							allRows.Append(rowInfo.X + "C");

							for (int x = 0; x < rowInfo.Width; x++)
							{
								RGB rgb = colors[x + rowInfo.X, y];
								if (rgb is null)
								{
									allRows.Append(" ");
								}
								else
								{
									allRows.Append(escapeStart);
									allRows.Append(rgb.R);
									allRows.Append(colorSeparator);
									allRows.Append(rgb.G);
									allRows.Append(colorSeparator);
									allRows.Append(rgb.B);
									allRows.Append(escapeEnd);
								}
							}
							allRows.Append(Environment.NewLine);
						}
					}
					else
					{
						allRows.Append(Environment.NewLine);
					}
				}




				// Clearing the console Using some p/invoking
				for (int i = 0; i < spritePositionsCopy.Count; i++)
				{
					COORD position = new COORD((short)spritePositionsCopy[i].X, (short)spritePositionsCopy[i].X);
					for (int y = 0; y < spritePositionsCopy[i].Height; y++)
					{
						FillConsoleOutputCharacter(
							GetStdHandle(StdHandle.OutputHandle),
							whiteSpace,
							spritePositionsCopy[i].Width,
							position,
							out int lpNumberOfCharsWritten
							);

						position.Y++;
					}
				}

				// Writing all the rows to the console
				WriteConsoleW(
					stdOutHandle,
					allRows,
					allRows.Length,
					out int charsWritten,
					IntPtr.Zero);

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

				// Destroying all gameobjects from destructionQueue
				DestroyGameObjects();

				// This is to avoid the console flickering randomly
				Thread.Sleep(10);
			}
		}

		internal static Queue<GameObject> destructionQueue = new Queue<GameObject>();
		public static List<SpriteDisplayer> RenderedGameObjects { internal get; set; } = new List<SpriteDisplayer>();

		static void DoNothing() {	}
		static Action updateCallBack = new Action(DoNothing);

		internal static void AddFrameSubscriber(Action method)
		{
			updateCallBack += method;
		}

		internal static void Unsubscribe(Action method)
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
