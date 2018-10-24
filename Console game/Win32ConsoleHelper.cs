using System;
using System.Runtime.InteropServices;
using static Console_game.NativeMethods;

namespace Console_game
{
	internal class Win32ConsoleHelper
	{
		public static void SetConsoleFontSize(ushort x, ushort y)
		{
			IntPtr outPutHandle = GetStdHandle(StdHandle.OutputHandle);

			if (outPutHandle == INVALID_HANDLE_VALUE)
			{
				Log.DefaultLogger.LogInfo($"Invalid handle {outPutHandle}");
				return;
			}

			CONSOLE_FONT_INFOEX ConsoleFontInfo = new CONSOLE_FONT_INFOEX();
			ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);
			GetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo);
			ConsoleFontInfo.dwFontSize.X = (short)x;
			ConsoleFontInfo.dwFontSize.Y = (short)y;

			SetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo);

			Marshal.FreeHGlobal(outPutHandle);
		}

		public enum ConsoleFont
		{
			Consolas, Courier_new, Lucida_console, MS_Gothic, NSim_Sun, Raster_Fonts, SimSun_ExtB
		}

		public static void SetConsoleFont(ConsoleFont newFont)
		{
			string newFontString;
			switch (newFont)
			{
				case ConsoleFont.Consolas:
					newFontString = "Consolas";
					break;
				case ConsoleFont.Courier_new:
					newFontString = "Courier New";
					break;
				case ConsoleFont.Lucida_console:
					newFontString = "Lucida Console";
					break;
				case ConsoleFont.MS_Gothic:
					newFontString = "MS Gothic";
					break;
				case ConsoleFont.NSim_Sun:
					newFontString = "NSim Sun";
					break;
				case ConsoleFont.Raster_Fonts:
					newFontString = "Raster Fonts";
					break;
				case ConsoleFont.SimSun_ExtB:
					newFontString = "SimSun-ExtB";
					break;
				default:
					Log.DefaultLogger.LogInfo($"Newfont went to default case: {newFont}");
					newFontString = "";
					return;
			}


			IntPtr outPutHandle = GetStdHandle(StdHandle.OutputHandle);

			if (outPutHandle == INVALID_HANDLE_VALUE)
			{
				Log.DefaultLogger.LogInfo($"Invalid handle {outPutHandle}");
				return;
			}

			CONSOLE_FONT_INFOEX ConsoleFontInfo = new CONSOLE_FONT_INFOEX();
			ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);
			GetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo);

			// For some reason, the terminal font can't be changed
			if (ConsoleFontInfo.FaceName == "Terminal")
			{
				Marshal.FreeHGlobal(outPutHandle);
				return;
			}

			ConsoleFontInfo.FaceName = newFontString;


			SetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo);

			Marshal.FreeHGlobal(outPutHandle);
		}
	}
}
