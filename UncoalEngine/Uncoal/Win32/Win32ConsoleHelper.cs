using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static Uncoal.Internal.NativeMethods;

namespace Uncoal.Internal
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

			if (!GetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo))
			{
				throw new Win32Exception();
			}

			ConsoleFontInfo.dwFontSize.X = (short)x;
			ConsoleFontInfo.dwFontSize.Y = (short)y;

			if (!SetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo))
			{
				throw new Win32Exception();
			} 
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
				throw new Win32Exception();
			}

			CONSOLE_FONT_INFOEX ConsoleFontInfo = new CONSOLE_FONT_INFOEX();
			ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);

			if (!GetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo))
			{
				throw new Win32Exception();
			}

			// For some reason, the terminal font can't be changed
			if (ConsoleFontInfo.FaceName == "Terminal")
				return;


			ConsoleFontInfo.FaceName = newFontString;


			if (!SetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo))
			{
				throw new Win32Exception();
			}

			if (!GetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo))
			{
				throw new Win32Exception();
			}
		}
	}
}
