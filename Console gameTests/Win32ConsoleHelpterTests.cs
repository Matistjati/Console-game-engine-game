using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Console_game.Tests
{
    [TestClass()]
    public class Win32ConsoleHelpterTests
    {
        Process cmd;

        [TestInitialize]
        public void SetUp()
        {
            cmd = Process.Start("cmd.exe");
        }

        [TestCleanup]
        public void TearDown()
        {
            cmd.WaitForExit(1);
            cmd.Dispose();
        }

        [DllImport("Kernel32.dll")]
        private static extern Int32 GetCurrentConsoleFontEx(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            ref _CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(StdHandle index);

        [DllImport("kernel32")]
        private static extern Int32 GetCurrentConsoleFont(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            ref _CONSOLE_FONT_INFO lpConsoleCurrentFont);

        [DllImport("kernel32")]
        private static extern _COORD GetConsoleFontSize(
            IntPtr hConsoleOutput,
            int nFont);

        private enum StdHandle
        {
            OutputHandle = -11
        }

        private readonly Vector2Int[] ConsoleFontSizes = new Vector2Int[]
            { new Vector2Int(4, 6), new Vector2Int(6, 8), new Vector2Int(8, 8), new Vector2Int(16, 8),
            new Vector2Int(5, 12), new Vector2Int(7, 12), new Vector2Int(8, 12), new Vector2Int(16, 12),
            new Vector2Int(12, 16), new Vector2Int(10, 18), new Vector2Int(4, 6), new Vector2Int(4, 6)};

        [TestMethod()]
        public void SetConsoleFontSizeNormalUsage()
        {
            for (int i = 0; i < ConsoleFontSizes.Length; i++)
            {
                ushort x = (ushort)ConsoleFontSizes[i].X;
                ushort y = (ushort)ConsoleFontSizes[i].Y;
                Win32ConsoleHelper.SetConsoleFontSize(x, y);
                _CONSOLE_FONT_INFO fontInfo = new _CONSOLE_FONT_INFO();
                GetCurrentConsoleFont(GetStdHandle(StdHandle.OutputHandle), false, ref fontInfo);
                _COORD fontSize = GetConsoleFontSize(GetStdHandle(StdHandle.OutputHandle), fontInfo.nFont);

                Assert.AreEqual(x, (ushort)fontSize.X);
                Assert.AreEqual(y, (ushort)fontSize.Y);
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

        [StructLayout(LayoutKind.Sequential)]
        private struct _CONSOLE_FONT_INFO
        {
            public int nFont;
            public _COORD dwFontSize;
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
