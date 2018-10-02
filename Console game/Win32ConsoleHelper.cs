using System;
using System.Runtime.InteropServices;

namespace Console_game
{
    internal class Win32ConsoleHelper
    {
        [DllImport("Kernel32.dll")]
        public static extern int SetConsoleTitle(string lpConsoleTitle);

        [DllImport("Kernel32.dll")]
        private static extern Int32 SetCurrentConsoleFontEx(
                    IntPtr hConsoleOutput,
                    bool bMaximumWindow,
                    ref _CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

        [DllImport("Kernel32.dll")]
        private static extern Int32 GetCurrentConsoleFontEx(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            ref _CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

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

            _CONSOLE_FONT_INFO_EX ConsoleFontInfo = new _CONSOLE_FONT_INFO_EX();
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

            unsafe
            {
                IntPtr outPutHandle = GetStdHandle(StdHandle.OutputHandle);

                if (outPutHandle == INVALID_HANDLE_VALUE)
                {
                    Log.defaultLogger.LogInfo($"Invalid handle {outPutHandle}");
                    return;
                }

                _CONSOLE_FONT_INFO_EX ConsoleFontInfo = new _CONSOLE_FONT_INFO_EX();
                ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);

                IntPtr ptr = new IntPtr(ConsoleFontInfo.FaceName);
                Marshal.Copy(newFontString.ToCharArray(), 0, ptr, newFontString.Length);

                SetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo);

                Marshal.FreeHGlobal(outPutHandle);
                Marshal.FreeHGlobal(ptr);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct _COORD
        {
            public short X;
            public short Y;

            public _COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        private const int LF_FACESIZE = 32;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private unsafe struct _CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public _COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;
            public fixed char FaceName[LF_FACESIZE];
        }
    }
}
