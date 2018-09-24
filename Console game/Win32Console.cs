using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Console_game
{
    class Win32Console
    {
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

        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(StdHandle index);

        public static void SetConsoleFontSize(short x, short y)
        {
            unsafe
            {
                IntPtr outPutHandle = GetStdHandle(StdHandle.OutputHandle);

                if (outPutHandle == INVALID_HANDLE_VALUE)
                {
                    throw new Exception("send help 3");
                }

                _CONSOLE_FONT_INFO_EX ConsoleFontInfo = new _CONSOLE_FONT_INFO_EX();
                ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);

                GetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo);
                ConsoleFontInfo.dwFontSize.X = x;
                ConsoleFontInfo.dwFontSize.Y = y;

                SetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo);

                Marshal.FreeHGlobal(outPutHandle);
            }
        }

        public struct ConsoleFonts
        {

        }

        public static void SetConsoleFont(string fontName = "Lucida Console")
        {
            unsafe
            {
                IntPtr outPutHandle = GetStdHandle(StdHandle.OutputHandle);

                if (outPutHandle == INVALID_HANDLE_VALUE)
                {
                    
                    throw new Exception("send help 4");
                }

                _CONSOLE_FONT_INFO_EX ConsoleFontInfo = new _CONSOLE_FONT_INFO_EX();
                ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);

                GetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo);
                IntPtr ptr = new IntPtr(ConsoleFontInfo.FaceName);
                Marshal.Copy(fontName.ToCharArray(), 0, ptr, fontName.Length);

                SetCurrentConsoleFontEx(outPutHandle, false, ref ConsoleFontInfo);
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
