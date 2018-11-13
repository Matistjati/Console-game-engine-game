using System;
using System.Globalization;

namespace Uncoal.Engine
{
	public struct CoordF
	{
		internal static readonly CoordF empty = new CoordF();

		public float X;

		public float Y;

		public CoordF(float x, float y)
		{
			X = x;
			Y = y;
		}

		public static explicit operator Coord(CoordF coordF)
		{
			return new Coord((int)coordF.X, (int)coordF.Y);
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
