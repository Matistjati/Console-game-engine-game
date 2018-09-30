using System;
using System.Text;
using System.Drawing;

namespace Console_game
{
    public class Map
    {
        public int seed { get; }
        public int mapSizeY { get; }
        public int mapSizeX { get; }
        public float[,] map { get; }

        private Vector2 _playerPosition;
        public Vector2 playerPosition
        {
            set
            {
                if (value.Y < 0 || value.Y >= mapSizeY || value.X < 0 || value.X >= mapSizeX)
                {
                    throw new ArgumentException("out of bounds");
                }
                _playerPosition = value;
            }
            get
            {
                return _playerPosition;
            }
        }

        private const int standardMapSizeX = 1200;
        private const int standardMapSizeY = 300;
        private const int standardScale = 1;
        private const int playerViewRangeX = 60;
        private const int playerViewRangeY = 15;

        public Map()
            : this(new Random().Next(1, int.MaxValue), standardMapSizeX, standardMapSizeY, standardScale) { }

        public Map(int seed)
            : this(seed, standardMapSizeX, standardMapSizeY, standardScale) { }

        public Map(int seed, int mapSizeX, int mapSizeY, int scale)
        {
            if (mapSizeY <= 0 || mapSizeX <= 0)
                throw new ArgumentException($"map must have a size greater than 0. X: {mapSizeX} Y: {mapSizeY} ");


            this.seed = seed;
            this.mapSizeY = mapSizeY;
            this.mapSizeX = mapSizeX;
            playerPosition = new Vector2(15, 15);
            map = Map_Generator.MakeMap(seed, mapSizeX, mapSizeY, scale);
        }

        public Rectangle GetSeenMap()
        {
            int upperLeftX = (int)(playerPosition.X - playerViewRangeX);
            int upperLeftY = (int)(playerPosition.Y - playerViewRangeY);

            return new Rectangle(
                upperLeftX > 0 ? upperLeftX : 0, upperLeftY > 0 ? upperLeftY : 0, 
                playerViewRangeX * 2 <= map.GetLength(0) ? playerViewRangeX * 2 : 0,
                playerViewRangeY * 2 <= map.GetLength(1) ? playerViewRangeY * 2 : 0);
        }

        public ConsoleColor[,] getPrintableMap()
        {
            Rectangle mapBoundaries = GetSeenMap();
            ConsoleColor[,] mapColors = new ConsoleColor[mapBoundaries.Width, mapBoundaries.Height];

            for (int y = 0; y < mapBoundaries.Height; ++y)
            {
                for (int x = 0; x < mapBoundaries.Width; ++x)
                {
                    float mapValue = this.map[x + mapBoundaries.X, y + mapBoundaries.Y];

                    ConsoleColor tileColor;
                    if (mapValue < -0.05)
                    {
                        tileColor = ConsoleColor.Blue;
                    }
                    else if (mapValue < 0.0)
                    {
                        tileColor = ConsoleColor.Yellow;
                    }
                    else if (mapValue < 1.0)
                    {
                        tileColor = ConsoleColor.Green;
                    }
                    else if (mapValue < 0.35)
                    {
                        tileColor = ConsoleColor.Gray;
                    }
                    else if (mapValue < 1.0)
                    {
                        tileColor = ConsoleColor.White;
                    }
                    else
                    {
                        tileColor = ConsoleColor.Blue;
                    }

                    mapColors[x, y] = tileColor;
                }
            }

            return mapColors;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(mapSizeX * mapSizeY);
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    stringBuilder.Append("\n" + map[y, x]);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
