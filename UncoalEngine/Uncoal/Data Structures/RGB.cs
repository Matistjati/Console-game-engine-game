using System.Drawing;

namespace Uncoal.Engine
{
	public class RGB
	{
		// Optimized for speed in my case
		// fields are faster than properties
		const string escapeStartRGB = "\x1b[38;2;";
		const string escapeEnd = "m█";
		const char colorSeparator = ';';
		public readonly string escapeSequence;

		public readonly bool isEmpty = false;

		public RGB(byte r, byte g, byte b)
		{
			if (r == 0 && g == 0 && b == 0)
			{
				this.isEmpty = true;
				this.escapeSequence = string.Empty;
			}
			else
			{
				escapeSequence = escapeStartRGB;
			 	escapeSequence += r;
				escapeSequence += colorSeparator;
				escapeSequence += g;
				escapeSequence += colorSeparator;
				escapeSequence += b;
				escapeSequence += escapeEnd;
			}
		}

		public RGB(Color color)
		{
			if (color.R == 0 && color.G == 0 && color.B == 0)
			{
				this.isEmpty = true;
				this.escapeSequence = string.Empty;
			}
			else
			{
				escapeSequence = escapeStartRGB;
				escapeSequence += color.R;
				escapeSequence += colorSeparator;
				escapeSequence += color.G;
				escapeSequence += colorSeparator;
				escapeSequence += color.B;
				escapeSequence += escapeEnd;
			}
		}
	}
}
