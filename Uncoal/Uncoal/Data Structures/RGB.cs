using System.Drawing;

namespace Uncoal.Engine
{
	class RGB
	{
		// Optimized for speed in my case
		// fields are faster than properties
		public readonly byte R;
		public readonly byte G;
		public readonly byte B;

		public readonly bool isEmpty = false;

		public RGB(byte r, byte g, byte b)
		{
			R = r;
			G = g;
			B = b;

			if (R == 0 && G == 0 && B == 0)
				isEmpty = true;
		}

		public RGB(Color color)
		{
			R = color.R;
			G = color.G;
			B = color.B;

			if (R == 0 && G == 0 && B == 0)
				isEmpty = true;
		}

		public override string ToString()
		{
			return $"R:{R}, G: {G}, B: {B}";
		}
	}
}
