using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using Uncoal.Engine;
using Uncoal.MapGenerating;

namespace Uncoal.Internal
{
	internal class Program
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

		static void GameSetup()
		{
			Win32ConsoleHelper.SetConsoleFontSize(1, 1);
			Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			Console.BufferWidth = Console.WindowWidth;
			Console.BufferHeight = Console.WindowHeight;
			Console.SetWindowPosition(0, 0);

			// Set the console's title to a preset gamename
			NativeMethods.SetConsoleTitle(ConfigurationManager.AppSettings["Game Name"]);

			// Set up input handlelers

			InternalInput.Start();

			GameObjectSetup();

			// Creating the necessary folders and files
			Directory.CreateDirectory("logs");
			using (StreamWriter x = File.AppendText("logs/log.txt")) { }

			// Activating escape sequences
			uint mode = 0;
			NativeMethods.GetConsoleMode(NativeMethods.GetStdHandle(NativeMethods.StdHandle.OutputHandle), ref mode);
			NativeMethods.SetConsoleMode(NativeMethods.GetStdHandle(NativeMethods.StdHandle.OutputHandle), mode | 0x4);

			// We won't have to deal with the console cursor
			Console.CursorVisible = false;

			GameObject.isStartUpPhase = false;
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
