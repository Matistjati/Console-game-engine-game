namespace Uncoal.Engine
{
	class SmallRectangle
	{
		public static readonly SmallRectangle empty = new SmallRectangle(0, 0, 0, 0);

		public short X;

		public short Y;

		public short Width;

		public short Height;

		public SmallRectangle(short x, short y, short width, short height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public override string ToString()
		{
			return $"X: {X} Y: {Y} W: {Width} H: {Height}";
		}
	}
}
