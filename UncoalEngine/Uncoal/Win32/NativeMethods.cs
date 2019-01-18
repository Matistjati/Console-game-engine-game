using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Uncoal.Internal
{
	internal static class NativeMethods
	{
		/////////////////////
		// Console handles //
		/////////////////////
		[DllImport("kernel32")]
		public static extern IntPtr GetStdHandle(StdHandle index);

		public enum StdHandle
		{
			OutputHandle = -11,
			InputHandle = -10,
			ErrorHandle = -12
		}

		public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

		/////////////////////////////
		// Common Data Structures ///
		/////////////////////////////

		[StructLayout(LayoutKind.Sequential)]
		public struct SMALL_RECT
		{
			public short Left;
			public short Top;
			public short Right;
			public short Bottom;
		}


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


		//////////////////////////////
		// Console write operations //
		//////////////////////////////
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool WriteConsoleW(
			IntPtr hConsoleOutput,
			StringBuilder lpBuffer,
			int nNumberOfCharsToWrite,
			out int lpNumberOfCharsWritten,
			IntPtr lpReservedMustBeNull);


		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool FillConsoleOutputCharacter(
			IntPtr hConsoleOutput,
			char character,
			int nLength,
			COORD dwWriteCoord,
			out int pNumCharsWritten);

		[DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool WriteConsoleOutput(
			IntPtr hConsoleOutput,
			/* This pointer is treated as the origin of a two-dimensional array of CHAR_INFO structures
			whose size is specified by the dwBufferSize parameter.*/
			[MarshalAs(UnmanagedType.LPArray), In] CHAR_INFO[,] lpBuffer,
			COORD dwBufferSize,
			COORD dwBufferCoord,
			ref SMALL_RECT lpWriteRegion);


		[StructLayout(LayoutKind.Sequential)]
		public struct CHAR_INFO
		{
			public char UnicodeChar;
			public CharAttribute Attributes;
		}

		[Flags]
		public enum CharAttribute : ushort
		{
			None = 0x0000,

			FOREGROUND_BLUE = 0x0001,

			FOREGROUND_GREEN = 0x0002,

			FOREGROUND_RED = 0x0004,

			ForegroundYellow = 0x0006,
	
			FOREGROUND_INTENSITY = 0x0008,

			BACKGROUND_BLUE = 0x0010,

			BACKGROUND_GREEN = 0x0020,

			BACKGROUND_RED = 0x0040,

			BackgroundYellow = 0x60,

			BACKGROUND_INTENSITY = 0x0080,

			/// <summary>
			/// Leading byte.
			/// </summary>
			COMMON_LVB_LEADING_BYTE = 0x0100,

			/// <summary>
			/// Trailing byte.
			/// </summary>
			COMMON_LVB_TRAILING_BYTE = 0x0200,

			/// <summary>
			/// Top horizontal
			/// </summary>
			COMMON_LVB_GRID_HORIZONTAL = 0x0400,

			/// <summary>
			/// Left vertical.
			/// </summary>
			COMMON_LVB_GRID_LVERTICAL = 0x0800,

			/// <summary>
			/// Right vertical.
			/// </summary>
			COMMON_LVB_GRID_RVERTICAL = 0x1000,

			/// <summary>
			/// Reverse foreground and background attribute.
			/// </summary>
			COMMON_LVB_REVERSE_VIDEO = 0x4000,

			/// <summary>
			/// Underscore.
			/// </summary>
			COMMON_LVB_UNDERSCORE = 0x8000,
		}

		///////////////////////////////
		// Console title management ///
		///////////////////////////////
		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool SetConsoleTitle(
			string lpConsoleTitle);


		[DllImport("kernel32")]
		public static extern bool GetConsoleTitle(
			[MarshalAs(UnmanagedType.LPArray)] byte[] lpConsoleTitle,
			uint nSize);







		/////////////////////
		// Font management //
		/////////////////////
		[DllImport("kernel32")]
		public static extern COORD GetConsoleFontSize(
			IntPtr hConsoleOutput,
			int nFont);


		[DllImport("kernel32")]
		public static extern bool GetCurrentConsoleFont(
			IntPtr hConsoleOutput,
			bool bMaximumWindow,
			ref CONSOLE_FONT_INFO lpConsoleCurrentFont);


		[StructLayout(LayoutKind.Sequential)]
		public struct CONSOLE_FONT_INFO
		{
			public int nFont;
			public COORD dwFontSize;
		}


		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool SetCurrentConsoleFontEx(
			IntPtr hConsoleOutput,
			bool bMaximumWindow,
			ref CONSOLE_FONT_INFOEX lpConsoleCurrentFontEx);


		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool GetCurrentConsoleFontEx(
			IntPtr hConsoleOutput,
			bool bMaximumWindow,
			ref CONSOLE_FONT_INFOEX lpConsoleCurrentFont);


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

		//////////////////////////////
		// Console input management //
		//////////////////////////////
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool ReadConsoleInput(
			IntPtr hConsoleInput,
			[Out] INPUT_RECORD[] lpBuffer,
			uint nLength,
			ref uint lpNumberOfEventsRead);


		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool WriteConsoleInput(
			IntPtr hConsoleInput,
			INPUT_RECORD[] lpBuffer,
			uint nLength,
			ref uint lpNumberOfEventsWritten);

		[DllImport("kernel32.dll")]
		public static extern bool FlushConsoleInputBuffer(
			IntPtr hConsoleInput);

		// Code analysis will bring errors, but things here work
		// This is most likely due to the union
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct INPUT_RECORD
		{
			public const ushort KEY_EVENT = 0x0001,
				MOUSE_EVENT = 0x0002,
				WINDOW_BUFFER_SIZE_EVENT = 0x0004;
			[FieldOffset(0)]
			public ushort EventType;

			// These are a union
			[FieldOffset(4)]
			public KEY_EVENT_RECORD KeyEvent;

			[FieldOffset(4)]
			public MOUSE_EVENT_RECORD MouseEvent;

			[FieldOffset(4)]
			public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;

			/*
            MENU_EVENT_RECORD MenuEvent;
            FOCUS_EVENT_RECORD FocusEvent;
            */
			// MSDN claims that these are used internally and shouldn't be used
			// https://docs.microsoft.com/en-us/windows/console/input-record-str

		}


		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSE_EVENT_RECORD
		{
			public COORD dwMousePosition;


			public const int DOUBLE_CLICK = 0x0002,
				MOUSE_HWHEELED = 0x0008,
				MOUSE_MOVED = 0x0001,
				MOUSE_WHEELED = 0x0004;

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

			public const int FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001,
				FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004,
				FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008,
				FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010,
				RIGHTMOST_BUTTON_PRESSED = 0x0002;

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

			[FieldOffset(12)]
			public uint dwControlKeyState;

			public const int CAPSLOCK_ON = 0x0080,
				ENHANCED_KEY = 0x0100,
				LEFT_ALT_PRESSED = 0x0002,
				LEFT_CTRL_PRESSED = 0x0008,
				NUMLOCK_ON = 0x0020,
				RIGHT_ALT_PRESSED = 0x0001,
				RIGHT_CTRL_PRESSED = 0x0004,
				SCROLLLOCK_ON = 0x0040,
				SHIFT_PRESSED = 0x0010;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOW_BUFFER_SIZE_RECORD
		{
			public COORD dwSize;
		}

		//////////////////////////
		// Console mode changes //
		//////////////////////////
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetConsoleMode(
			IntPtr hConsoleInput,
			ref ConsoleMode lpMode);


		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleMode(
			IntPtr hConsoleInput,
			ConsoleMode dwMode);


		public enum ConsoleMode
		{
			ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
			ENABLE_MOUSE_INPUT = 0x0010,
			ENABLE_QUICK_EDIT_MODE = 0x0040,
			ENABLE_EXTENDED_FLAGS = 0x0080,
			ENABLE_ECHO_INPUT = 0x0004,
			ENABLE_WINDOW_INPUT = 0x0008 //more
		}
	}
}
