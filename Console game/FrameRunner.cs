using System;
using System.Collections.Generic;
using System.Drawing;
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

		static Coord displaySize;
		static readonly Color emptyColor = Color.FromArgb(0, 0, 0, 0);

		internal static void DestroyGameObjects()
		{
			// The only way to do kill an objects is to kill all references to it
			for (int i = 0; i < destructionQueue.Count; i++)
			{
				GameObject gameObject = destructionQueue.Dequeue();
				List<Component> componentsToRemove = new List<Component>();
				foreach (Component componentToDestroy in updateCallBack.Keys.Where(component => component.gameObject == gameObject))
				{
					componentsToRemove.Add(componentToDestroy);
				}
				for (int o = 0; o < componentsToRemove.Count; o++)
				{
					updateCallBack.Remove(componentsToRemove[i]);
				}

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

			displaySize = new Coord((uint)Console.WindowWidth, (uint)Console.WindowHeight);

			run = true;
			while (run)
			{
				// Time Calculations
				GameObject._timeDelta = (float)(DateTime.Now - lastFrameCall).TotalSeconds;

				GameObject._time = (float)(DateTime.Now - start).TotalSeconds;

				lastFrameCall = DateTime.Now;

				// Setting the public input api
				Input.UpdateInput();

				// Calling update
				foreach (KeyValuePair<Component, MethodInfo> method in updateCallBack)
				{
					method.Key.Invoke(method.Value);
				}


				// Each member represents an x-row on the console
				List<StringBuilder> rows = new List<StringBuilder>((int)displaySize.Y);


				for (int i = 0; i < RenderedGameObjects.Count; i++)
				{
					// Only render the visible ones
					if (!RenderedGameObjects[i].IsVisible)
						continue;

					// Caching the position of the object to be rendered 
					Coord position = (Coord)RenderedGameObjects[i].physicalState.Position;

					// Checking that the sprite is on-screen
					if (position.X > displaySize.X && position.Y > displaySize.Y)
						continue;

					// Caching the size of the sprite
					Coord colorMapSize = new Coord(RenderedGameObjects[i].ColorMap.GetLength(0), RenderedGameObjects[i].ColorMap.GetLength(0));

					// Assuring we won't go out of bounds
					if (colorMapSize.X > displaySize.X)
						colorMapSize.SetX(displaySize.X);

					if (colorMapSize.Y > displaySize.Y)
						colorMapSize.SetY(displaySize.Y);

					// Displaychar is always of length one, but char has trouble representing unicode chars
					// Try to fix somehow? Lack of knowledge?
					string displayChar = RenderedGameObjects[i].PrintedChar;

					for (int x = 0; x < colorMapSize.X; x++)
					{
						StringBuilder currentRow = new StringBuilder((int)position.X);

						// Adding the spacing according to the sprite's position
						// Around 2x faster than string.Concat(Enumerable.Repeat(...))
						for (int whiteSpaces = 0; whiteSpaces < position.X; whiteSpaces++)
						{
							currentRow.Append(" ");
						}

						for (int y = 0; y < colorMapSize.Y; y++)
						{
							Color color = RenderedGameObjects[i].ColorMap[y, x];
							if (color != emptyColor)
							{
								currentRow.Append($"\x1b[38;2;{color.R};{color.G};{color.B}m" + displayChar);
							}
							else
							{
								currentRow.Append(" ");
							}
						}

						currentRow.Append(Environment.NewLine);

						rows.Add(currentRow);
					}
				}

				IntPtr stdOutHandle = GetStdHandle(StdHandle.OutputHandle);

				// Joining all the rows
				// This works because we append a newline onto the end of each row
				StringBuilder allRows = new StringBuilder();
				foreach (StringBuilder row in rows)
				{
					allRows.Append(row);
				}

				// Writing all the rows to the console
				Console.Clear();
				WriteConsoleW(
					stdOutHandle,
					allRows,
					allRows.Length,
					out int charsWritten,
					IntPtr.Zero);

				// Destroying all gameobjects from destructionQueue
				DestroyGameObjects();

				// This is to avoid the console flickering randomly
				Thread.Sleep(10);
			}
		}

		internal static Queue<GameObject> destructionQueue = new Queue<GameObject>();
		public static List<SpriteDisplayer> RenderedGameObjects { internal get; set; } = new List<SpriteDisplayer>();


		public static Dictionary<Component, MethodInfo> updateCallBack = new Dictionary<Component, MethodInfo>();

		internal static void AddFrameSubscriber(KeyValuePair<Component, MethodInfo> method)
		{
			if (!updateCallBack.Keys.Contains(method.Key))
			{
				updateCallBack.Add(method.Key, method.Value);
			}
		}

		internal static void AddFrameSubscriber(Dictionary<Component, MethodInfo> method)
		{
			foreach (KeyValuePair<Component, MethodInfo> methodInfo in method)
			{
				if (!updateCallBack.Keys.Contains(methodInfo.Key))
				{
					updateCallBack.Add(methodInfo.Key, methodInfo.Value);
				}
			}
		}

		internal static void AddFrameSubscriber(MethodInfo method, object instance)
		{
			if (!updateCallBack.Keys.Contains(instance))
			{
				updateCallBack.Add((Component)instance, method);
			}
		}

		internal static void Unsubscribe(Component component)
		{
			if (updateCallBack.Keys.Contains(component))
			{
				updateCallBack.Remove(component);
			}
		}

		public static void UnsubscribeAll()
		{
			updateCallBack.Clear();
		}
	}
}
