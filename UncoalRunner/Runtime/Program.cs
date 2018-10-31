using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using Uncoal.Engine;
using Uncoal.MapGenerating;
using Uncoal.Internal;

namespace Uncoal.Runner
{
	public class Program
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
			// Setting console settings
			// The console cursor will look weird with all our write operations
			Console.CursorVisible = false;

			string[] fontSize;
			try
			{
				// Setting the font size according to Console Font Size in app.config
				fontSize = ConfigurationManager.AppSettings["Console Font Size"].Split(';');
			}
			catch (NullReferenceException exception)
			{
				throw new NullReferenceException("Whomst've removed my Console Font Size key in app.config", exception);
			}


			try
			{
				Win32ConsoleHelper.SetConsoleFontSize(
					ushort.Parse(fontSize[0]),  // X
					ushort.Parse(fontSize[1])); // Y
			}
			catch (FormatException exception)
			{
				throw new FormatException($"X or Y in App.config/Console Font Size was an invalid number." +
					$" X was \"{fontSize[0]}\", y was \"{fontSize[1]}\"", exception);
			}


			// Setting the window and buffer size to the max possible
			Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			Console.BufferWidth = Console.WindowWidth;
			Console.BufferHeight = Console.WindowHeight;

			try
			{
				FrameRunner.framesBetweenDraws = int.Parse(ConfigurationManager.AppSettings["FramesBetweenRenders"]);
			}
			catch (FormatException)
			{
				throw new FormatException($"FramesBetweenRenders in app.config was not a valid number" +
					$" FramesBetweenRenders was {ConfigurationManager.AppSettings["FramesBetweenRenders"]}");
			}
			catch (NullReferenceException exception)
			{
				throw new NullReferenceException("Whomst've removed my FramesBetweenRenders key in app.config", exception);
			}

			try
			{
				// Set the console's title to a preset gamename
				NativeMethods.SetConsoleTitle(ConfigurationManager.AppSettings["Game Name"]);
			}
			catch (NullReferenceException exception)
			{
				throw new NullReferenceException("Whomst've removed my Game Name key in app.config", exception);
			}


			// Activating escape sequences
			uint mode = 0;
			NativeMethods.GetConsoleMode(NativeMethods.GetStdHandle(NativeMethods.StdHandle.OutputHandle), ref mode);
			NativeMethods.SetConsoleMode(NativeMethods.GetStdHandle(NativeMethods.StdHandle.OutputHandle), mode | 0x4); // 0x4 is escape sequences

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

		static void Main(string[] args)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();


			Console.ReadKey(true);
			//Console.ReadKey(true);
			//Console.ReadKey(true);
			//Console.ReadKey(true);

			GameSetup();

			Console.SetBufferSize(1200, 300);
			Win32ConsoleHelper.SetConsoleFontSize(20, 20);
			//Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
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
