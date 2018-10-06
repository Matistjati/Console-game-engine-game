using System;
using System.IO;
using System.Numerics;

namespace Console_game
{
    class Program
    {
        private static float total;
        private static void TestTimeAccuracy()
        {
            total += GameObject.TimeDelta;
            Console.Clear();
            Console.Write($"time: {GameObject.Time}\ntimedelta total: {total}\ntimedelta: {GameObject.TimeDelta}\ndifference: {total -GameObject.Time}");
        }

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
            // Set the console's title to a preset gamename
            NativeMethods.SetConsoleTitle(Globals.gameName);
            Win32ConsoleHelper.SetConsoleFontSize(18, 18);

            // Set up input handlelers
            InternalInput.Start();

            // Getting all classes deriving from gameobject and getting update and start methods
            ReflectiveHelper<GameObject> gameObjectChildren = new ReflectiveHelper<GameObject>();
            Globals.GameMethodSignature frameSubscribers = gameObjectChildren.GetMethodsByString("update");
            Globals.GameMethodSignature gameStartup = gameObjectChildren.GetMethodsByString("start");
            gameStartup.Invoke();
            frameSubscribers += TestTimeAccuracy;
            FrameRunner.AddFrameSubscriber(frameSubscribers);

            // Testing
            frameSubscribers += TestTimeAccuracy;

            // Creating the necessary folders and files
            Directory.CreateDirectory("logs");
            using (StreamWriter x = File.AppendText("logs/log.txt")) { }

            // Starting
            FrameRunner.Start();
        }

        static void Main(string[] args)
        {
            InternalInput.Start();
            Console.ReadKey(false);
            Console.ReadKey(false);
            Console.ReadKey(false);
            Console.ReadKey(false);

            //GameSetup();

            Console.SetBufferSize(1200, 300);
            //Console.SetWindowSize(599, 149);
            Map thisMap = new Map(5000, 1500)
            {
                PlayerViewRangeX = 600,
                PlayerViewRangeY = 150
            };

            PrintMap(thisMap);

            Console.ReadKey(false);
        }

        private const string blockChar = "█";
        static void PrintMap(Map map)
        {
            ConsoleColor[,] mapColors = map.GetPrintableMap(new Vector2(15, 15));
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
