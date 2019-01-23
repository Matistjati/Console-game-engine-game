using System;
using System.Drawing;
using System.Linq;

namespace Uncoal.Internal
{
	struct FastRgb
	{
		public readonly byte R;
		public readonly byte B;
		public readonly byte G;

		public FastRgb(byte r, byte g, byte b)
		{
			R = r;
			B = b;
			G = g;
		}

		public static explicit operator FastRgb(Color other)
		{
			return new FastRgb(other.R, other.B, other.G);
		}
	}

	public static class ConsoleColorHelper
	{
		static readonly ConsoleColor[] consoleColors;
		static readonly FastRgb[] consoleColorsRGB;

		static ConsoleColorHelper()
		{
			consoleColors = Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>().ToArray();
			consoleColorsRGB = new FastRgb[consoleColors.Length];
			for (int i = 0; i < consoleColors.Length; i++)
			{
				string name = Enum.GetName(typeof(ConsoleColor), consoleColors[i]);
				Color color = Color.FromName(name == "DarkYellow" ? "Orange" : name); // Bug fix, darkyellow is weird
				consoleColorsRGB[i] = (FastRgb)color;
			}
		}

	 	public static ConsoleColor ClosestConsoleColor(byte r, byte g, byte b)
		{
			ConsoleColor ret = 0;
			double rr = r, gg = g, bb = b, delta = double.MaxValue;

			// Gets the total color difference and returns the consolecolor with the least difference
			for (int i = 0; i < consoleColors.Length; i++)
			{
				FastRgb currentColor = consoleColorsRGB[i];

				double redDelta = currentColor.R - rr;
				double greenDelta = currentColor.G - gg;
				double blueDelta = currentColor.B - bb;
				double deltaSum = redDelta * redDelta + greenDelta * greenDelta + blueDelta * blueDelta;

				if (deltaSum == 0.0)
					return consoleColors[i];

				if (deltaSum < delta)
				{
					delta = deltaSum;
					ret = consoleColors[i];
				}
			}

			return ret;
		}
	}
}
