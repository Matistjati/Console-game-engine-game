using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace Console_game.Tests
{
    [TestClass()]
    public class Map_GeneratorTests
    {
        Random randomGen = new Random();

        [TestMethod()]
        public void MakeMapSuccessNormal()
        {
            float[,] gameMap = Map_Generator.MakeMap(
                seed: randomGen.Next(),
                mapSize: new Point(randomGen.Next(1, 100), randomGen.Next(1, 25)),
                scale: 1);

            // Assuring that the map has been filled
            Assert.IsTrue(gameMap[randomGen.Next(0, gameMap.GetLength(0)), randomGen.Next(0, gameMap.GetLength(1))] != 0 &&
                          gameMap[randomGen.Next(0, gameMap.GetLength(0)), randomGen.Next(0, gameMap.GetLength(1))] != 0);
        }

        [TestMethod()]
        public void MakeMapSuccessNegativeSeed()
        {
            float[,] gameMap = Map_Generator.MakeMap(
                seed: randomGen.Next(-1000, -1),
                mapSize: new Point(randomGen.Next(1, 100), randomGen.Next(1, 25)),
                scale: 1);

            // Assuring that the map has been filled
            Assert.IsTrue(gameMap[randomGen.Next(0, gameMap.GetLength(0)), randomGen.Next(0, gameMap.GetLength(1))] != 0 &&
                          gameMap[randomGen.Next(0, gameMap.GetLength(0)), randomGen.Next(0, gameMap.GetLength(1))] != 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MakeMapNegativeXException()
        {
            float[,] gameMap = Map_Generator.MakeMap(
                seed: randomGen.Next(),
                mapSize: new Point(-20, randomGen.Next(1, 25)),
                scale: 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MakeMapNegativeYException()
        {
            float[,] gameMap = Map_Generator.MakeMap(
                seed: randomGen.Next(), 
                mapSize: new Point(randomGen.Next(1, 25), -50),
                scale: 1);
        }
    }
}