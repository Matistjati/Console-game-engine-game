using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uncoal.Engine;
using Uncoal.Internal;
using static Uncoal.Internal.NativeMethods;

namespace Uncoal.Tests
{
	[TestClass()]
	public class Win32ConsoleHelpterTests
	{
		static Process cmd;

		[ClassInitialize()]
		public static void SetUp(TestContext context)
		{
			cmd = Process.Start("cmd.exe");
		}

		[ClassCleanup]
		public static void TearDown()
		{
			cmd.Kill();
		}

		private readonly Coord[] ConsoleFontSizes = new Coord[]
			{ new Coord(4, 6), new Coord(6, 8), new Coord(8, 8), new Coord(16, 8),
			new Coord(5, 12), new Coord(7, 12), new Coord(8, 12), new Coord(16, 12),
			new Coord(12, 16), new Coord(10, 18), new Coord(4, 6), new Coord(4, 6)};

		[TestMethod()]
		public void SetConsoleFontSizeNormalUsage()
		{
			Win32ConsoleHelper.SetConsoleFont(Win32ConsoleHelper.ConsoleFont.Lucida_console);


			for (int i = 0; i < ConsoleFontSizes.Length; i++)
			{
				ushort x = (ushort)ConsoleFontSizes[i].X;
				ushort y = (ushort)ConsoleFontSizes[i].Y;
				Win32ConsoleHelper.SetConsoleFontSize(x, y);
				CONSOLE_FONT_INFO fontInfo = new CONSOLE_FONT_INFO();
				GetCurrentConsoleFont(GetStdHandle(StdHandle.OutputHandle), false, ref fontInfo);
				COORD fontSize = GetConsoleFontSize(GetStdHandle(StdHandle.OutputHandle), fontInfo.nFont);

				Assert.AreEqual(x, (ushort)fontSize.X);
				Assert.AreEqual(y, (ushort)fontSize.Y);
			}
		}

		// Random strings to test in SetConsoleTitleNormalUsage
		private static readonly string[] consoleTestNames = new string[] { "memory", "whether", "industrial", "reach", "car", "off",
			"toward", "oxygen", "friendly", "draw", "window", "aboard",
			"bend", "been", "track", "central", "war", "keep",
			"personal", "gas", "proud", "involved", "smooth", "tightly",
			"swing", "soldier", "fur", "roof", "bring", "plates",
			"compound", "accept", "concerned", "track", "bright", "you",
			"storm", "listen", "advice", "earlier", "influence", "car" };

		[TestMethod()]
		public void SetConsoleTitleNormalUsage()
		{
			Random rnd = new Random();
			string consoleTestName = consoleTestNames[rnd.Next(0, consoleTestNames.Length)];
			SetConsoleTitle(consoleTestName);

			byte[] receiver = new byte[consoleTestName.Length + 1];
			GetConsoleTitle(receiver, (uint)receiver.Length);


			Assert.AreEqual(consoleTestName,
			// Converting the byte array to string and slicing away the null terminator
			Encoding.Default.GetString(receiver).Substring(0, receiver.Length - 1));
		}

		private const string consolasString = "Consolas";
		private const string luicidaConsoleString = "Lucida Console";

		[TestMethod()]
		public void SetConsoleFontNormalUsage()
		{
			// Only testing the options "guaranteed" to be installed
			Win32ConsoleHelper.SetConsoleFont(Win32ConsoleHelper.ConsoleFont.Consolas);
			CONSOLE_FONT_INFOEX consoleFontInfo = new CONSOLE_FONT_INFOEX();
			consoleFontInfo.cbSize = (uint)Marshal.SizeOf(consoleFontInfo);
			GetCurrentConsoleFontEx(GetStdHandle(StdHandle.OutputHandle), false, ref consoleFontInfo);

			// For some reason, the terminal font can't be changed
			if (consoleFontInfo.FaceName == "Terminal")
			{
				Assert.IsTrue(true);
				return;
			}

			else
				Assert.AreEqual(consolasString, consoleFontInfo.FaceName);

			CONSOLE_FONT_INFOEX consoleFontInfo2 = new CONSOLE_FONT_INFOEX();
			consoleFontInfo2.cbSize = (uint)Marshal.SizeOf(consoleFontInfo2);

			Win32ConsoleHelper.SetConsoleFont(Win32ConsoleHelper.ConsoleFont.Lucida_console);
			GetCurrentConsoleFontEx(GetStdHandle(StdHandle.OutputHandle), false, ref consoleFontInfo2);
			Assert.AreEqual(luicidaConsoleString, consoleFontInfo2.FaceName);
		}
	}
}
