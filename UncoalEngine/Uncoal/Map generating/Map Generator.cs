using System;
using Uncoal.Engine;

namespace Uncoal.MapGenerating
{
	public class Map_Generator
	{
		public static float[,] MakeMap(int seed, Coord mapSize, float scale)
		{
			if (scale <= 0)
				throw new ArgumentOutOfRangeException($"Scale: {scale} was negative");

			float[,] map = new float[mapSize.X, mapSize.Y];

			FastNoise perlin = new FastNoise(seed);

			//perlin.SetNoiseType(FastNoise.NoiseType.Perlin);

			for (int y = 0; y < map.GetLength(1); y++)
			{
				for (int x = 0; x < map.GetLength(0); x++)
				{
					float xCoord = x * scale;
					float yCoord = y * scale;

					float val = perlin.GetPerlin(xCoord, yCoord);

					map[x, y] = val;
				}
			}

			return map;
		}
	}
}
