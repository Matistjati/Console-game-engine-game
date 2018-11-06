using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using Uncoal.Engine;
using Uncoal.MapGenerating;
using Uncoal.Internal;
using System.Runtime.InteropServices;

namespace Uncoal.Runner
{
	public class Program
	{
#if DEBUG
		private static void TestInputAccuracy()
		{
			if (Input.GetKeyDown('a'))
			{
				Console.Write("down\n");
			}
			if (Input.GetKeyHeld('a'))
			{
				Console.Write("holding\n");
			}
			if (Input.GetKeyUp('a'))
			{
				Console.Write("up\n");
			}
		}
#endif
		static void GameObjectSetup()
		{
			// Note:
			// Lots of things here get assigned to null
			// This is to ensure that gameobjects can be destroyed and have their memory freed up during runtime

			// Getting all classes deriving from gameobject and getting update and start methods
			ReflectiveHelper<GameObject> gameObjectChildren = new ReflectiveHelper<GameObject>();
			List<GameObject> gameObjects = gameObjectChildren.GetTInstanceNonPrefab();


			// Adding physicalstate to all gameObjects
			foreach (GameObject gameObject in gameObjects)
			{
				gameObject.AddComponent(gameObject.physicalState);
			}

			// Setting up all gameobjects who we might want to render
			foreach (GameObject gameObject in gameObjects)
			{
				if (gameObject.GetComponent<SpriteDisplayer>() is SpriteDisplayer sprite && sprite.IsInitialized)
				{
					FrameRunner.RenderedGameObjects.Add(sprite);
					// Leave no references hanging, see top of method
					sprite = null;
				}
			}

			// Invoking all start methods on our GameObjects
			foreach (GameObject gameObject in gameObjects)
			{
				foreach (Component component in gameObject.components)
				{
					if (ReflectiveHelper<Type>.TryGetMethodFromComponent(component, "start", out MethodInfo method))
					{
						method.Invoke(component, null);
						// Leave no references hanging, see top of method
						method = null;
					}
				}
			}


			FrameRunner.AddFrameSubscriber(gameObjectChildren.GetComponentAction("update"));

			// Killing references
			gameObjectChildren.TChildren = null;
			gameObjectChildren = null;
			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i] = null;
			}
			gameObjects = null;
		}

		static void GameSetup(ushort fontSizeX, ushort fontSizeY, int framesBetweenDraws, string gameName)
		{
			// Setting console settings
			// The console cursor will look weird with all our write operations
			Console.CursorVisible = false;

			// Setting the font size according to what was passed in start
			Win32ConsoleHelper.SetConsoleFontSize(fontSizeX, fontSizeY);

			// Setting the window and buffer size to the max possible
			Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			Console.BufferWidth = Console.WindowWidth;
			Console.BufferHeight = Console.WindowHeight;

			// Setting how many frames we want to run between drawing to the console according to what was passed in start
			FrameRunner.framesBetweenDraws = framesBetweenDraws;

			// Set the console's title to a preset gamename
			NativeMethods.SetConsoleTitle(gameName);


			// Activating escape sequences

			IntPtr bufferHandle = NativeMethods.GetStdHandle(NativeMethods.StdHandle.OutputHandle);

			if (bufferHandle == NativeMethods.INVALID_HANDLE_VALUE)
			{
				Log.DefaultLogger.LogError($"Win32Error calling GetStdHandle (invalid handle): {Marshal.GetLastWin32Error()}");
			}

			uint mode = 0;

			bool succ = NativeMethods.GetConsoleMode(bufferHandle, ref mode);

			mode = mode | 0x4; // 0x4 is escape sequences

			bool success = NativeMethods.SetConsoleMode(bufferHandle, mode); 

			if (!success)
			{
				Log.DefaultLogger.LogError($"Win32Error calling setconsolemode: {Marshal.GetLastWin32Error()}");
			}

			// Set up input handlelers
			InternalInput.Start();

			// Setting up all gameobjects
			GameObjectSetup();

			// Creating the necessary folders and files
			Directory.CreateDirectory("logs");
			using (StreamWriter x = File.AppendText("logs/log.txt")) { }

			// Starting
			FrameRunner.Run();
		}

		public static void Start(ushort fontSizeX, ushort fontSizeY, int framesBetweenDraws, string gameName)
		{
#if DEBUG
			Console.ReadKey(true);
#endif


			GameSetup(fontSizeX, fontSizeY, framesBetweenDraws, gameName);

#if DEBUG
			Console.SetBufferSize(1200, 300);
			Win32ConsoleHelper.SetConsoleFontSize(20, 20);
			//Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			Map thisMap = new Map(5000, 1500)
			{
				PlayerViewRange = new Coord(600, 150)
			};



			PrintMap(thisMap);

			Console.ReadKey(true);
#endif
		}

#if DEBUG
		private const char blockChar = '█';
		static void PrintMap(Map map)
		{
			ConsoleColor[,] mapColors = map.GetPrintableMap(new Coord(15, 15));
			for (int y = 0; y < mapColors.GetLength(0); y++)
			{
				for (int x = 0; x < mapColors.GetLength(1); x++)
				{
					Console.ForegroundColor = mapColors[y, x];
					Console.Write(blockChar);
				}
			}
		}
#endif
	}
}