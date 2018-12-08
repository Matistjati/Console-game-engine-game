using System.Globalization;

namespace Uncoal.Engine
{
	public struct Coord
	{
		internal static readonly Coord empty = new Coord();

		public int X;

		public int Y;

		public Coord(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static explicit operator CoordF(Coord coord)
		{
			return new CoordF(coord.X, coord.Y);
		}

		public void Clamp(Coord min, Coord max)
		{
			int X = this.X;
			X = (X > max.X) ? max.X : X;
			X = (X < min.X) ? min.X : X;

			int Y = this.Y;
			Y = (Y > max.Y) ? max.Y : Y;
			Y = (Y < min.Y) ? min.Y : Y;

			this.X = X;
			this.Y = Y;
		}

		public static bool operator ==(Coord left, Coord right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator !=(Coord left, Coord right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Coord))
				return false;
			Coord coord = (Coord)obj;
			return coord.X == this.X && coord.Y == this.Y;
		}

		public override int GetHashCode()
		{
			return X ^ Y;
		}

		public override string ToString()
		{
			return $"X: {X.ToString(CultureInfo.CurrentCulture)} , Y: {Y.ToString(CultureInfo.CurrentCulture)}";
		}
	}
}
