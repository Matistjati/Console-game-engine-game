using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uncoal.Engine;
using Uncoal.MapGenerating;

namespace Uncoal.Tests
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
				mapSize: new Coord(randomGen.Next(1, 100), randomGen.Next(1, 25)),
				scale: 1);

			// Assuring that the map has been filled
			// Assuring that the map has been filled
			int unfilledTiles = 0;
			int checkedTiles = 30;
			for (int i = 0; i < checkedTiles; i++)
			{
				float value = gameMap[
					randomGen.Next(gameMap.GetLength(0)),
					randomGen.Next(gameMap.GetLength(1))];

				if (value == 0)
					unfilledTiles++;
			}
			Assert.AreNotEqual(unfilledTiles, checkedTiles);
		}

		[TestMethod()]
		public void MakeMapSuccessNegativeSeed()
		{
			float[,] gameMap = Map_Generator.MakeMap(
				seed: randomGen.Next(-1000, -1),
				mapSize: new Coord(randomGen.Next(1, 100), randomGen.Next(1, 25)),
				scale: 1);

			// Assuring that the map has been filled
			int unfilledTiles = 0;
			int checkedTiles = 30;
			for (int i = 0; i < checkedTiles; i++)
			{
				float value = gameMap[
					randomGen.Next(gameMap.GetLength(0)),
					randomGen.Next(gameMap.GetLength(1))];

				if (value == 0)
					unfilledTiles++;
			}
			Assert.AreNotEqual(unfilledTiles, checkedTiles);
		}
	}
}