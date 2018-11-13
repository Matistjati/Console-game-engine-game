namespace Uncoal.Engine
{
	class SmallRectangle
	{
		public static readonly SmallRectangle empty = new SmallRectangle(0, 0, 0, 0);

		public readonly short X;

		public readonly short Y;

		public readonly short Width;

		public readonly short Height;

		public SmallRectangle(short x, short y, short width, short height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}
	}
}
