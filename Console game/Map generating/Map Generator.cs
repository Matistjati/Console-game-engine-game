using System;

namespace Console_game
{
    public class Map_Generator
    {
        public static float[,] MakeMap(int seed, int mapSizeX, int mapSizeY, float scale)
        {
            if (mapSizeX <= 0 || mapSizeY <= 0)
                throw new ArgumentException($"A negative mapsize was entered: X: {mapSizeX} y: {mapSizeY}");
            if (scale <= 0)
                throw new ArgumentException($"Scale: {scale} was negative");

            float[,] map = new float[mapSizeX, mapSizeY];

            FastNoise perlin = new FastNoise(seed);
            perlin.SetNoiseType(FastNoise.NoiseType.Perlin);
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)

                {
                    float xCoord = (float)x / mapSizeX * scale;
                    float yCoord = (float)y / mapSizeY * scale;

                    map[x, y] = perlin.GetValue(xCoord, yCoord);
                }
            }

            return map;
        }
    }
}
