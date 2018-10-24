using System;
using System.Globalization;

namespace Console_game
{
	public struct CoordF
	{
		public static readonly CoordF empty = new CoordF();

		public float X { get; private set; }

		public float Y { get; private set; }

		public void Set(float X, float Y)
		{
			if (X < 0)
				throw new ArgumentOutOfRangeException($"x must be greater than 0. X was {X}");
			if (Y < 0)
				throw new ArgumentOutOfRangeException($"y must be greater than 0. X was {Y}");
			this.X = X;
			this.Y = Y;
		}

		public void SetX(float X)
		{
			if (X < 0)
				throw new ArgumentOutOfRangeException($"x must be greater than 0. X was {X}");
			this.X = X;
		}

		public void SetY(float Y)
		{
			if (Y < 0)
				throw new ArgumentOutOfRangeException($"x must be greater than 0. X was {Y}");
			this.Y = Y;
		}

		public CoordF(float x, float y)
		{
			if (x < 0)
				throw new ArgumentOutOfRangeException($"x must be greater than 0. X was {x}");
			if (y < 0)
				throw new ArgumentOutOfRangeException($"y must be greater than 0. X was {y}");

			X = x;
			Y = y;
		}

		public static explicit operator Coord(CoordF coordF)
		{
			return new Coord((uint)coordF.X, (uint)coordF.Y);
		}

		public void Clamp(Coord min, Coord max)
		{
			float X = this.X;
			X = (X > max.X) ? max.X : X;
			X = (X < min.X) ? min.X : X;

			float Y = this.Y;
			Y = (Y > max.Y) ? max.Y : Y;
			Y = (Y < min.Y) ? min.Y : Y;

			this.X = X;
			this.Y = Y;
		}

		public static bool operator ==(CoordF left, CoordF right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator !=(CoordF left, CoordF right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CoordF))
				return false;
			CoordF coord = (CoordF)obj;
			return coord.X == this.X && coord.Y == this.Y;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode();
		}

		public override string ToString()
		{
			return $"X: {X.ToString(CultureInfo.CurrentCulture)} , Y: {Y.ToString(CultureInfo.CurrentCulture)}";
		}
	}
}
