using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace Console_game.Tests
{
    [TestClass()]
    public class MapTests
    {
        static Random rnd = new Random();

        [TestMethod()]
        public void MapTestEmptyConstructorSucces()
        {
            // This one takes a while since the default constructor size is 600 x 150
            Map gameMap = new Map();

            // Assuring that the map has been filled
            int width = gameMap.map.GetLength(0);
            int height = gameMap.map.GetLength(1);
            for (int i = 0; i < 50; i++)
            {
                float randomPosition1 = gameMap.map[
                    rnd.Next(0, width),
                    rnd.Next(0, height)];
                Assert.IsTrue(randomPosition1 < 1 && randomPosition1 > -1);
            }


            // Variables being set
            Assert.IsTrue(gameMap.Seed != 0);
            Assert.IsTrue(gameMap.MapSizeX != 0);
            Assert.IsTrue(gameMap.MapSizeY != 0);
        }

        [TestMethod()]
        public void MapTestSeedConstructorSucces()
        {
            // This one takes a while since the default constructor size is 600 x 150
            Map gameMap = new Map(rnd.Next(1, int.MaxValue));

            // Assuring that the map has been filled with proper values
            int width = gameMap.map.GetLength(0);
            int height = gameMap.map.GetLength(1);
            for (int i = 0; i < 50; i++)
            {
                float randomPosition1 = gameMap.map[
                    rnd.Next(0, width),
                    rnd.Next(0, height)];
                Assert.IsTrue(randomPosition1 < 1 && randomPosition1 > -1);
            }

            // Variables being set
            Assert.IsTrue(gameMap.Seed != 0);
            Assert.IsTrue(gameMap.MapSizeX != 0);
            Assert.IsTrue(gameMap.MapSizeY != 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MapTestSeedConstructorExceptionNegativeX()
        {
            Map gameMap = new Map(rnd.Next(), -20, rnd.Next(1, 250), 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MapTestSeedConstructorExceptionNegativeY()
        {
            Map gameMap = new Map(rnd.Next(), rnd.Next(1, 250), -20, 1);
        }

        [TestMethod()]
        public void GetPrintableMapTest()
        {
            Map gameMap = new Map(rnd.Next(), 30, 30, 1);
            ConsoleColor[,] mapColors = gameMap.GetPrintableMap(new Vector2Int(15, 15));
            Rectangle mapCutOut = gameMap.GetSeenMap(new Vector2Int(15, 15));

            for (int x = 0; x < mapCutOut.Width; ++x)
            {
                for (int y = 0; y < mapCutOut.Height; ++y)
                {
                    float mapValue = gameMap.map[x, y];

                    ConsoleColor tileColor;
                    if (mapValue < -0.75)
                    {
                        tileColor = ConsoleColor.Blue;
                    }
                    else if (mapValue < -0.5)
                    {
                        tileColor = ConsoleColor.Yellow;
                    }
                    else if (mapValue < -0.25)
                    {
                        tileColor = ConsoleColor.Green;
                    }
                    else if (mapValue < 0)
                    {
                        tileColor = ConsoleColor.Gray;
                    }
                    else if (mapValue < 0.25)
                    {
                        tileColor = ConsoleColor.White;
                    }
                    else
                    {
                        tileColor = ConsoleColor.Blue;
                    }

                    Assert.AreEqual(tileColor, mapColors[x, y]);
                }
            }
        }

        // Getseenmaptests beyond this point
        Map testMap = new Map(30, 30);

        [TestMethod()]
        public void GetSeenMapNormalUsage()
        {
            Vector2Int position = new Vector2Int(15, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapRightEdge()
        {
            Vector2Int position = new Vector2Int(29, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapLeftEdge()
        {
            Vector2Int position = new Vector2Int(0, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(0, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapTop()
        {
            Vector2Int position = new Vector2Int(15, 0);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(0, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapBottom()
        {
            Vector2Int position = new Vector2Int(15, 29);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsRight()
        {
            Vector2Int position = new Vector2Int(60, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsLeft()
        {
            Vector2Int position = new Vector2Int(-20, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(0, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsTop()
        {
            Vector2Int position = new Vector2Int(15, -20);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(0, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsBottom()
        {
            Vector2Int position = new Vector2Int(15, 60);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }
    }
}