using System;
using System.Runtime.InteropServices;

namespace Console_game
{
    internal class Win32ConsoleHelper
    {
        [DllImport("Kernel32.dll")]
        public static extern int SetConsoleTitle(
            string lpConsoleTitle);

        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern Int32 SetCurrentConsoleFontEx(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            ref CONSOLE_FONT_INFOEX lpConsoleCurrentFontEx);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern Int32 GetCurrentConsoleFontEx(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            ref CONSOLE_FONT_INFOEX lpConsoleCurrentFont);

        private enum StdHandle
        {
            OutputHandle = -11
        }

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(StdHandle index);

        public static void SetConsoleFontSize(ushort x, ushort y)
        {
            IntPtr outPutHandle = GetStdHandle(StdHandle.OutputHandle);

            if (outPutHandle == INVALID_HANDLE_VALUE)
            {
                Log.defaultLogger.LogInfo($"Invalid handle {outPutHandle}");
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
                    Log.defaultLogger.LogInfo($"Newfont went to default case: {newFont}");
                    newFontString = "";
                    return;
            }


            IntPtr outPutHandle = GetStdHandle(StdHandle.OutputHandle);

            if (outPutHandle == INVALID_HANDLE_VALUE)
            {
                Log.defaultLogger.LogInfo($"Invalid handle {outPutHandle}");
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


            int i = SetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo);

            Marshal.FreeHGlobal(outPutHandle);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;

            public COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CONSOLE_FONT_INFOEX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }
    }
}
