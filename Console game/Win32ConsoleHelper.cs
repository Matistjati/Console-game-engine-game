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
    }

    internal static class NativeMethods
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int SetConsoleTitle(
            string lpConsoleTitle);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Int32 SetCurrentConsoleFontEx(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            ref CONSOLE_FONT_INFOEX lpConsoleCurrentFontEx);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Int32 GetCurrentConsoleFontEx(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            ref CONSOLE_FONT_INFOEX lpConsoleCurrentFont);

        public enum StdHandle
        {
            OutputHandle = -11
        }

        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32")]
        public static extern IntPtr GetStdHandle(StdHandle index);

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
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
        public struct CONSOLE_FONT_INFOEX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct INPUT_RECORD
        {
            public const ushort KEY_EVENT = 0x0001,
                MOUSE_EVENT = 0x0002,
                WINDOW_BUFFER_SIZE_EVENT = 0x0004; //more


            public ushort EventType;

            public KEY_EVENT_RECORD KeyEvent;

            public MOUSE_EVENT_RECORD MouseEvent;

            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
            /*
            and:
             MENU_EVENT_RECORD MenuEvent;
             FOCUS_EVENT_RECORD FocusEvent;
             */
        }

        public struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;

            public const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001,
                FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004,
                FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008,
                FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010,
                RIGHTMOST_BUTTON_PRESSED = 0x0002;
            public uint dwButtonState;

            public const int CAPSLOCK_ON = 0x0080,
                ENHANCED_KEY = 0x0100,
                LEFT_ALT_PRESSED = 0x0002,
                LEFT_CTRL_PRESSED = 0x0008,
                NUMLOCK_ON = 0x0020,
                RIGHT_ALT_PRESSED = 0x0001,
                RIGHT_CTRL_PRESSED = 0x0004,
                SCROLLLOCK_ON = 0x0040,
                SHIFT_PRESSED = 0x0010;
            public uint dwControlKeyState;

            public const int DOUBLE_CLICK = 0x0002,
                MOUSE_HWHEELED = 0x0008,
                MOUSE_MOVED = 0x0001,
                MOUSE_WHEELED = 0x0004;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct KEY_EVENT_RECORD
        {
            [FieldOffset(0)]
            public bool bKeyDown;
            [FieldOffset(4)]
            public ushort wRepeatCount;
            [FieldOffset(6)]
            public ushort wVirtualKeyCode;
            [FieldOffset(8)]
            public ushort wVirtualScanCode;
            [FieldOffset(10)]
            public char UnicodeChar;
            [FieldOffset(10)]
            public byte AsciiChar;

            public const int CAPSLOCK_ON = 0x0080,
                ENHANCED_KEY = 0x0100,
                LEFT_ALT_PRESSED = 0x0002,
                LEFT_CTRL_PRESSED = 0x0008,
                NUMLOCK_ON = 0x0020,
                RIGHT_ALT_PRESSED = 0x0001,
                RIGHT_CTRL_PRESSED = 0x0004,
                SCROLLLOCK_ON = 0x0040,
                SHIFT_PRESSED = 0x0010;
            [FieldOffset(12)]
            public uint dwControlKeyState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_FONT_INFO
        {
            public int nFont;
            public COORD dwFontSize;
        }

        public struct WINDOW_BUFFER_SIZE_RECORD
        {
            public COORD dwSize;
        }

        public const uint STD_INPUT_HANDLE = unchecked((uint)-10),
            STD_OUTPUT_HANDLE = unchecked((uint)-11),
            STD_ERROR_HANDLE = unchecked((uint)-12);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);

        public const uint ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008; //more

        [DllImport("kernel32.dll")]
        public static extern bool GetConsoleMode(
            IntPtr hConsoleInput,
            ref uint lpMode);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(
            IntPtr hConsoleInput,
            uint dwMode);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleInput(
            IntPtr hConsoleInput,
            [Out] INPUT_RECORD[] lpBuffer,
            uint nLength,
            ref uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleInput(
            IntPtr hConsoleInput,
            INPUT_RECORD[] lpBuffer,
            uint nLength,
            ref uint lpNumberOfEventsWritten);

        [DllImport("kernel32")]
        public static extern Int32 GetConsoleTitle(
            [MarshalAs(UnmanagedType.LPArray)] byte[] lpConsoleTitle,
            uint nSize);

        [DllImport("kernel32")]
        public static extern COORD GetConsoleFontSize(
            IntPtr hConsoleOutput,
            int nFont);

        [DllImport("kernel32")]
        public static extern Int32 GetCurrentConsoleFont(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            ref CONSOLE_FONT_INFO lpConsoleCurrentFont);
    }
}
