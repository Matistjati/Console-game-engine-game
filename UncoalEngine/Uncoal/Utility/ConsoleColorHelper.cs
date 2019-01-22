using System;
using System.Drawing;
using System.Linq;

namespace Uncoal.Internal
{
	public static class ConsoleColorHelper
	{
		static ConsoleColor[] consoleColors;
		static Color[] consoleColorsRGB;

		static ConsoleColorHelper()
		{
			consoleColors = Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>().ToArray();
			consoleColorsRGB = new Color[consoleColors.Length];
			for (int i = 0; i < consoleColors.Length; i++)
			{
				string name = Enum.GetName(typeof(ConsoleColor), consoleColors[i]);
				Color color = Color.FromName(name == "DarkYellow" ? "Orange" : name); // Bug fix, darkyellow is weird
				consoleColorsRGB[i] = color;
			}
		}

	 	public static ConsoleColor ClosestConsoleColor(byte r, byte g, byte b)
		{
			ConsoleColor ret = 0;
			double rr = r, gg = g, bb = b, delta = double.MaxValue;

			// Gets the total color difference and returns the consolecolor with the least difference
			for (int i = 0; i < consoleColors.Length; i++)
			{
				Color currentColor = consoleColorsRGB[i];

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
