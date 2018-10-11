using System;
using System.Drawing;
using System.Text;

namespace Console_game
{
    public class Map
    {
        public int Seed { get; }
        public float[,] map { get; }

        private static readonly Point standardMapSize = new Point(600, 150);
        private const float standardScale = 1f;

        // Do not modify, it will break things as Point is a struct
        private Point _mapSize = new Point();

        public Point MapSize => _mapSize;

        private Point _playerViewRange = new Point(14, 14);

        public Point PlayerViewRange
        {
            get { return _playerViewRange; }
            set
            {
                if (value.X <= 2)
                    throw new ArgumentException($"X was too small. X was {value}");
                if (value.Y <= 2)
                    throw new ArgumentException($"Y was too small. Y was {value}");

                if (value.X * 2 > MapSize.X)
                    throw new ArgumentException($"Y was too big. {value} * 2 > {MapSize.X}(mapsize)");
                if (value.Y * 2 > MapSize.Y)
                    throw new ArgumentException($"Y was too big. {value} * 2 > {MapSize.Y}(mapsize)");

                _playerViewRange = value;
            }
        }

        public Map()
            : this(new Random().Next(1, int.MaxValue), standardMapSize.X, standardMapSize.Y, standardScale) { }

        public Map(int seed)
            : this(seed, standardMapSize.X, standardMapSize.Y, standardScale) { }

        public Map(int mapSizeX, int mapSizeY)
            : this(new Random().Next(1, int.MaxValue), mapSizeX, mapSizeY, standardScale) { }

        public Map(int seed, int mapSizeX, int mapSizeY, float scale)
        {
            if (mapSizeY <= 0 || mapSizeX <= 0)
                throw new ArgumentException($"map must have a size greater than 0. X: {mapSizeX} Y: {mapSizeY} ");
            if (scale <= 0)
                throw new ArgumentException($"Scale: {scale} was negative");

            this.Seed = seed;
            this._mapSize.Y = mapSizeY;
            this._mapSize.X = mapSizeX;
            maxMapPosition = new Point(MapSize.X, MapSize.X);
            map = Map_Generator.MakeMap(seed, _mapSize, scale);
        }

        private readonly Point minMapPosition = new Point(0, 0);
        private readonly Point maxMapPosition;

        public Rectangle GetSeenMap(Point position)
        {
            // Setting the position to the top left
            position.X = (position.X - PlayerViewRange.X > 0) ? position.X - PlayerViewRange.X : 0;
            position.Y = (position.Y - PlayerViewRange.Y > 0) ? position.Y - PlayerViewRange.Y : 0;

            // Assuring that we're inside the map
            position = position.Clamp(minMapPosition, maxMapPosition);

            int width = PlayerViewRange.X * 2;
            int heigth = PlayerViewRange.Y * 2;

            if (position.X + width > MapSize.X)
            {
                position.X -= position.X + width - MapSize.X;
            }

            if (position.Y + heigth > MapSize.Y)
            {
                position.Y -= position.Y + heigth - MapSize.Y;
            }

            return new Rectangle(
                x: position.X,
                y: position.Y,
                width: width,
                height: heigth);
        }

        public ConsoleColor[,] GetPrintableMap(Point playerPosition)
        {
            Rectangle mapBoundaries = GetSeenMap(playerPosition);
            ConsoleColor[,] mapColors = new ConsoleColor[mapBoundaries.Width, mapBoundaries.Height];

            for (int x = 0; x < mapBoundaries.Width; ++x)
            {
                for (int y = 0; y < mapBoundaries.Height; ++y)
                {
                    double mapValue = this.map[x, y];

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
            StringBuilder stringBuilder = new StringBuilder(MapSize.X * MapSize.Y);
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
