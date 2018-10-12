using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace Console_game.Tests
{
    [TestClass()]
    public class MapTests
    {
        static Random randomGen = new Random();

        [TestMethod()]
        public void MapTestEmptyConstructorSucces()
        {
            // This one takes a while since the default constructor size is 600 x 150
            // Also, the method placed physically at the top is always faster for some reason
            Map gameMap = new Map();

            // Assuring that the map has been filled
            int width = gameMap.map.GetLength(0);
            int height = gameMap.map.GetLength(1);
            for (int i = 0; i < 50; i++)
            {
                float randomMapValue = gameMap.map[
                    randomGen.Next(0, width),
                    randomGen.Next(0, height)];
                Assert.IsTrue(randomMapValue < 1 && randomMapValue > -1);
            }

            // Variables being set
            Assert.IsTrue(gameMap.Seed != 0);
            Assert.IsTrue(gameMap.MapSize.X != 0);
            Assert.IsTrue(gameMap.MapSize.Y != 0);
        }

        [TestMethod()]
        public void MapTestSeedConstructorSucces()
        {
            // This one takes a while since the default constructor size is 600 x 150
            Map gameMap = new Map(randomGen.Next());

            // Assuring that the map has been filled with proper values
            int width = gameMap.map.GetLength(0);
            int height = gameMap.map.GetLength(1);
            for (int i = 0; i < 50; i++)
            {
                float randomMapValue = gameMap.map[
                    randomGen.Next(0, width),
                    randomGen.Next(0, height)];
                Assert.IsTrue(randomMapValue < 1 && randomMapValue > -1);
            }

            // Variables being set
            Assert.IsTrue(gameMap.Seed != 0);
            Assert.IsTrue(gameMap.MapSize.X != 0);
            Assert.IsTrue(gameMap.MapSize.Y != 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MapTestSeedConstructorExceptionNegativeX()
        {
            Map gameMap = new Map(randomGen.Next(), -20, randomGen.Next(1, 250), 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MapTestSeedConstructorExceptionNegativeY()
        {
            Map gameMap = new Map(randomGen.Next(), randomGen.Next(1, 250), -20, 1);
        }

        [TestMethod()]
        public void GetPrintableMapTest()
        {
            Map gameMap = new Map(randomGen.Next(), 30, 30, 1);
            ConsoleColor[,] mapColors = gameMap.GetPrintableMap(new Point(15, 15));
            Rectangle mapCutOut = gameMap.GetSeenMap(new Point(15, 15));

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
            Point position = new Point(15, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);

            // Testing we're inside bounds
            Assert.IsFalse(seenMap.Width + seenMap.X > testMap.MapSize.X * 2);
            Assert.IsFalse(seenMap.Height + seenMap.Y > testMap.MapSize.Y * 2);

            // Testing that the seen map is equal to that of the viewrange
            Assert.AreEqual(testMap.PlayerViewRange.X * 2, seenMap.Width + seenMap.X);
            Assert.AreEqual(testMap.PlayerViewRange.Y * 2, seenMap.Height + seenMap.Y);
        }

        [TestMethod()]
        public void GetSeenMapRightEdge()
        {
            Point position = new Point(29, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);

            Assert.IsFalse(seenMap.Width + seenMap.X > testMap.MapSize.X * 2);
            Assert.IsFalse(seenMap.Height + seenMap.Y > testMap.MapSize.Y * 2);

            Assert.AreEqual(testMap.PlayerViewRange.X * 2, seenMap.Width + seenMap.X);
            Assert.AreEqual(testMap.PlayerViewRange.Y * 2, seenMap.Height + seenMap.Y);
        }

        [TestMethod()]
        public void GetSeenMapLeftEdge()
        {
            Point position = new Point(0, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(testMap.PlayerViewRange.X * 2, seenMap.X + seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRange.Y * 2, seenMap.Y + seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapTop()
        {
            Point position = new Point(15, 0);

            Rectangle seenMap = testMap.GetSeenMap(position);

            Assert.IsFalse(seenMap.Width + seenMap.X > testMap.MapSize.X * 2);
            Assert.IsFalse(seenMap.Height + seenMap.Y > testMap.MapSize.Y * 2);

            Assert.AreEqual(testMap.PlayerViewRange.X * 2, seenMap.Width + seenMap.X);
            Assert.AreEqual(testMap.PlayerViewRange.Y * 2, seenMap.Height + seenMap.Y);
        }

        [TestMethod()]
        public void GetSeenMapBottom()
        {
            Point position = new Point(15, 29);

            Rectangle seenMap = testMap.GetSeenMap(position);

            Assert.IsFalse(seenMap.Width + seenMap.X > testMap.MapSize.X * 2);
            Assert.IsFalse(seenMap.Height + seenMap.Y > testMap.MapSize.Y * 2);

            Assert.AreEqual(testMap.PlayerViewRange.X * 2, seenMap.Width + seenMap.X);
            Assert.AreEqual(testMap.PlayerViewRange.Y * 2, seenMap.Height + seenMap.Y);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsRight()
        {
            Point position = new Point(60, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);

            Assert.IsFalse(seenMap.X < 0);

            Assert.IsFalse(seenMap.Width + seenMap.X > testMap.MapSize.X * 2);
            Assert.IsFalse(seenMap.Height + seenMap.Y > testMap.MapSize.Y * 2);

            Assert.AreEqual(testMap.PlayerViewRange.X * 2, seenMap.Width + seenMap.X);
            Assert.AreEqual(testMap.PlayerViewRange.Y * 2, seenMap.Height + seenMap.Y);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsLeft()
        {
            Point position = new Point(-20, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);

            Assert.IsFalse(seenMap.X < 0);

            Assert.IsFalse(seenMap.Width + seenMap.X > testMap.MapSize.X * 2);
            Assert.IsFalse(seenMap.Height + seenMap.Y > testMap.MapSize.Y * 2);

            Assert.AreEqual(testMap.PlayerViewRange.X * 2, seenMap.Width + seenMap.X);
            Assert.AreEqual(testMap.PlayerViewRange.Y * 2, seenMap.Height + seenMap.Y);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsTop()
        {
            Point position = new Point(15, -20);

            Rectangle seenMap = testMap.GetSeenMap(position);

            Assert.IsFalse(seenMap.Y < 0);

            Assert.IsFalse(seenMap.Width + seenMap.X > testMap.MapSize.X * 2);
            Assert.IsFalse(seenMap.Height + seenMap.Y > testMap.MapSize.Y * 2);

            Assert.AreEqual(testMap.PlayerViewRange.X * 2, seenMap.Width + seenMap.X);
            Assert.AreEqual(testMap.PlayerViewRange.Y * 2, seenMap.Height + seenMap.Y);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsBottom()
        {
            Point position = new Point(15, 60);

            Rectangle seenMap = testMap.GetSeenMap(position);

            Assert.IsFalse(seenMap.Y < 0);

            Assert.IsFalse(seenMap.Width + seenMap.X > testMap.MapSize.X * 2);
            Assert.IsFalse(seenMap.Height + seenMap.Y > testMap.MapSize.Y * 2);

            Assert.AreEqual(testMap.PlayerViewRange.X * 2, seenMap.Width + seenMap.X);
            Assert.AreEqual(testMap.PlayerViewRange.Y * 2, seenMap.Height + seenMap.Y);
        }
    }
}