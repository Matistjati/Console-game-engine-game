using System;
using System.Drawing;

namespace Console_game
{
    public class Map_Generator
    {
        public static float[,] MakeMap(int seed, Point mapSize, float scale)
        {
            if (mapSize.X <= 0)
                throw new ArgumentException($"The map size must be grater than 0. mapSizeX {mapSize.X} <= 0");
            if (mapSize.Y <= 0)
                throw new ArgumentException($"The map size must be grater than 0. mapSizeX {mapSize.Y} <= 0");
            if (scale <= 0)
                throw new ArgumentException($"Scale: {scale} was negative");

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
