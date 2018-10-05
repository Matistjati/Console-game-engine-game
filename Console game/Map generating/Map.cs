using System;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace Console_game
{
    public class Map
    {
        public int Seed { get; }
        public int MapSizeY { get; }
        public int MapSizeX { get; }
        public float[,] map { get; }

        private const int standardMapSizeX = 600;
        private const int standardMapSizeY = 150;
        private const float standardScale = 0.001f;

        private int _playerViewRangeX = 14;
        private int _playerViewRangeY = 14;

        public int PlayerViewRangeX
        {
            get { return _playerViewRangeX; }
            set
            {
                if (value <= 2)
                    throw new ArgumentException($"X was too small. X was {value}");
                if (value > MapSizeX)
                    throw new ArgumentException($"Y was too big. {value} > {MapSizeX}(mapsize)");
                _playerViewRangeX = value;
            }
        }

        public int PlayerViewRangeY
        {
            get { return _playerViewRangeY; }
            set
            {
                if (value <= 2)
                    throw new ArgumentException($"Y was too small. Y was {value}");
                if (value > MapSizeY)
                    throw new ArgumentException($"Y was too big. {value} > {MapSizeY}(mapsize)");
                _playerViewRangeY = value;
            }
        }

        public Map()
            : this(new Random().Next(1, int.MaxValue), standardMapSizeX, standardMapSizeY, standardScale) { }

        public Map(int seed)
            : this(seed, standardMapSizeX, standardMapSizeY, standardScale) { }

        public Map(int mapSizeX, int mapSizeY)
            : this(new Random().Next(1, int.MaxValue), mapSizeX, mapSizeY, standardScale) { }

        public Map(int seed, int mapSizeX, int mapSizeY, float scale)
        {
            if (mapSizeY <= 0 || mapSizeX <= 0)
                throw new ArgumentException($"map must have a size greater than 0. X: {mapSizeX} Y: {mapSizeY} ");

            this.Seed = seed;
            this.MapSizeY = mapSizeY;
            this.MapSizeX = mapSizeX;
            map = Map_Generator.MakeMap(seed, mapSizeX, mapSizeY, scale);
        }

        public Rectangle GetSeenMap(Vector2 position)
        {
            position.X = (int)position.X;
            position.Y = (int)position.Y;

            int x = (int)(position.X - PlayerViewRangeX);
            int y = (int)(position.Y - PlayerViewRangeY);

            return new Rectangle(
                x: x > 0 ? x : 0, // Negative value protection
                y: y > 0 ? y : 0, // Negative value protection
                width: PlayerViewRangeX * 2 <= map.GetLength(0) ? PlayerViewRangeX * 2 : map.GetLength(0),
                height: PlayerViewRangeY * 2 <= map.GetLength(1) ? PlayerViewRangeY * 2 : map.GetLength(1));
        }

        public ConsoleColor[,] GetPrintableMap(Vector2 playerPosition)
        {
            Rectangle mapBoundaries = GetSeenMap(playerPosition);
            ConsoleColor[,] mapColors = new ConsoleColor[mapBoundaries.Width, mapBoundaries.Height];

            for (int x = 0; x < mapBoundaries.Width; ++x)
                for (int y = 0; y < mapBoundaries.Height; ++y)
                {
                    float mapValue = this.map[x + mapBoundaries.X, y + mapBoundaries.Y];

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

                    mapColors[x, y] = tileColor;
                }

            return mapColors;
        }

        public override string ToString()
        {
#if (DEBUG)
            StringBuilder stringBuilder = new StringBuilder(MapSizeX * MapSizeY);
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    stringBuilder.Append("\n" + map[y, x]);
                }
            }
            return stringBuilder.ToString();
#else
            return "Use printer class for printing the map";
#endif
        }
    }
}
