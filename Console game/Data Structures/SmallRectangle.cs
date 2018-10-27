namespace Console_game
{
	class SmallRectangle
	{
		public static readonly SmallRectangle empty = new SmallRectangle(0, 0, 0, 0);

		public readonly ushort X;

		public readonly ushort Y;

		public readonly ushort Width;

		public readonly ushort Height;

		public SmallRectangle(ushort x, ushort y, ushort width, ushort height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}
	}
}
