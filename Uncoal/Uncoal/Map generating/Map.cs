using System;
using System.Drawing;
using System.Text;
using Uncoal.Engine;

namespace Uncoal.MapGenerating
{
	public class Map
	{
		private static Random randomGen = new Random();

		public int Seed { get; }
		public float[,] map { get; }

		private static readonly Coord standardMapSize = new Coord(600, 150);
		private const float standardScale = 1f;

		public Coord MapSize { get; } = new Coord();

		private Coord _playerViewRange = new Coord(14, 14);

		public Coord PlayerViewRange
		{
			get { return _playerViewRange; }
			set
			{
				if (value.X <= 2)
					throw new ArgumentOutOfRangeException($"X was too small. X was {value}");
				if (value.Y <= 2)
					throw new ArgumentOutOfRangeException($"Y was too small. Y was {value}");

				if (value.X * 2 > MapSize.X)
					throw new ArgumentOutOfRangeException($"Y was too big. {value} * 2 > {MapSize.X}(mapsize)");
				if (value.Y * 2 > MapSize.Y)
					throw new ArgumentOutOfRangeException($"Y was too big. {value} * 2 > {MapSize.Y}(mapsize)");

				_playerViewRange = value;
			}
		}

		public Map()
			: this(1, standardMapSize.X, standardMapSize.Y, standardScale) { }

		public Map(int seed)
			: this(seed, standardMapSize.X, standardMapSize.Y, standardScale) { }

		public Map(uint mapSizeX, uint mapSizeY)
			: this(randomGen.Next(), mapSizeX, mapSizeY, standardScale) { }

		public Map(int seed, uint mapSizeX, uint mapSizeY, float scale)
		{
			if (scale <= 0)
				throw new ArgumentOutOfRangeException($"Scale: {scale} was negative");

			this.Seed = seed;
			this.MapSize = new Coord(mapSizeY, mapSizeX);
			maxMapPosition = new Coord(MapSize.X, MapSize.X);
			map = Map_Generator.MakeMap(seed, MapSize, scale);
		}

		private readonly Coord minMapPosition = Coord.empty;
		private readonly Coord maxMapPosition;

		public Rectangle GetSeenMap(Coord position)
		{
			// Setting the position to the top left
			position.SetX((position.X - PlayerViewRange.X > 0) ? position.X - PlayerViewRange.X : 0);
			position.SetY((position.Y - PlayerViewRange.Y > 0) ? position.Y - PlayerViewRange.Y : 0);

			// Assuring that we're inside the map
			position.Clamp(minMapPosition, maxMapPosition);

			uint width = PlayerViewRange.X * 2;
			uint heigth = PlayerViewRange.Y * 2;

			if (position.X + width > MapSize.X)
			{
				position.SetX(position.X - (position.X + width - MapSize.X));
			}

			if (position.Y + heigth > MapSize.Y)
			{
				position.SetY(position.Y - (position.Y + heigth - MapSize.Y));
			}

			return new Rectangle(
				x: (int)((width + position.X > PlayerViewRange.X * 2) ? position.X - (width + position.X - PlayerViewRange.X * 2) : position.X),
				y: (int)((heigth + position.Y > PlayerViewRange.Y * 2) ? position.Y - (heigth + position.Y - PlayerViewRange.Y * 2) : position.Y),
				width: (int)width,
				height: (int)heigth);
		}

		public ConsoleColor[,] GetPrintableMap(Coord playerPosition)
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
			StringBuilder stringBuilder = new StringBuilder((int)(MapSize.X * MapSize.Y));
			for (int y = 0; y < map.GetLength(0); y++)
			{
				for (int x = 0; x < map.GetLength(1); x++)
				{
					stringBuilder.Append("\n" + map[y, x]);
				}
			}
			return stringBuilder.ToString();
#else
            return "Use GetPrintableMap for printing the map";
#endif
		}
	}
}
