using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Console_game
{
	class Program
	{
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

		static void GameSetup()
		{
			Win32ConsoleHelper.SetConsoleFontSize(10, 10);
			Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			Console.BufferWidth = Console.WindowWidth;
			Console.BufferHeight = Console.WindowHeight;
			Console.SetWindowPosition(0, 0);

			// Set the console's title to a preset gamename
			NativeMethods.SetConsoleTitle(ConfigurationManager.AppSettings["Game Name"]);

			// Set up input handlelers

			InternalInput.Start();

			// Declaring new scope to ensure that gameObjects and gameObjectChildren gets collected
			{
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
						}
					}
				}

				Dictionary<Component, MethodInfo> methodAndComponent = gameObjectChildren.GetComponentMethodAndInstance("update");

				FrameRunner.AddFrameSubscriber(methodAndComponent);
			}


			// Creating the necessary folders and files
			Directory.CreateDirectory("logs");
			using (StreamWriter x = File.AppendText("logs/log.txt")) { }

			// Activating escape sequences
			uint mode = 0;
			NativeMethods.GetConsoleMode(NativeMethods.GetStdHandle(NativeMethods.StdHandle.OutputHandle), ref mode);
			NativeMethods.SetConsoleMode(NativeMethods.GetStdHandle(NativeMethods.StdHandle.OutputHandle), mode | 0x4);

			// We won't have to deal with the white boi
			Console.CursorVisible = false;


			// Starting
			FrameRunner.Run();
		}

		static void Main(string[] args)
		{
			Console.ReadKey(true);
			//Console.ReadKey(true);
			//Console.ReadKey(true);
			//Console.ReadKey(true);

			GameSetup();

			Console.SetBufferSize(1200, 300);
			//Console.SetWindowSize(599, 149);
			Map thisMap = new Map(5000, 1500)
			{
				PlayerViewRange = new Coord(600, 150)
			};



			PrintMap(thisMap);

			Console.ReadKey(true);
		}

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
	}
}
