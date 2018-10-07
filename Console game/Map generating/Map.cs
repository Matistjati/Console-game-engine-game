using System;
using System.Drawing;
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
                if (value * 2 > MapSizeX)
                    throw new ArgumentException($"Y was too big. {value} * 2 > {MapSizeX}(mapsize)");
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
                if (value * 2 > MapSizeY)
                    throw new ArgumentException($"Y was too big. {value} * 2 > {MapSizeY}(mapsize)");
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
            maxMapPosition = new Vector2Int(MapSizeX, MapSizeX);
            map = Map_Generator.MakeMap(seed, mapSizeX, mapSizeY, scale);
        }

        private readonly Vector2Int minMapPosition = new Vector2Int(0, 0);
        private readonly Vector2Int maxMapPosition;

        public Rectangle GetSeenMap(Vector2Int position)
        {
            // Setting the position to the top left

            position.X = (position.X - PlayerViewRangeX > 0) ? position.X - PlayerViewRangeX : 0;
            position.Y = (position.Y - PlayerViewRangeY > 0) ? position.Y - PlayerViewRangeY : 0;

            // Assuring that we're inside the map
            position.Clamp(minMapPosition, maxMapPosition);

            int width = PlayerViewRangeX * 2;
            int heigth = PlayerViewRangeY * 2;

            if (position.X + width > MapSizeX)
            {
                position.X -= position.X + width - MapSizeX;
            }

            if (position.Y + heigth > MapSizeY)
            {
                position.Y -= position.Y + heigth - MapSizeY;
            }

            return new Rectangle(
                x: position.X,
                y: position.Y,
                width: width,
                height: heigth);
        }

        public ConsoleColor[,] GetPrintableMap(Vector2Int playerPosition)
        {
            Rectangle mapBoundaries = GetSeenMap(playerPosition);
            ConsoleColor[,] mapColors = new ConsoleColor[mapBoundaries.Width, mapBoundaries.Height];

            for (int x = 0; x < mapBoundaries.Width; ++x)
            {
                for (int y = 0; y < mapBoundaries.Height; ++y)
                {
                    float mapValue = this.map[x, y];

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
