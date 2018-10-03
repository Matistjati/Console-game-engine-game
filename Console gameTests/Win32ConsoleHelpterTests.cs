using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace Console_game.Tests
{
    [TestClass()]
    public class Win32ConsoleHelpterTests
    {
        static List<Process> cmd = new List<Process>();

        [TestInitialize]
        public void SetUp()
        {
            cmd.Add(Process.Start("cmd.exe"));
        }

        [TestCleanup]
        public void TearDown()
        {
            for (int i = 0; i < cmd.Count; i++)
            {
                cmd[i].Kill();
            }
            cmd.Clear();
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

        [DllImport("kernel32")]
        private static extern Int32 GetConsoleTitle(
            [MarshalAs(UnmanagedType.LPArray)] byte[] lpConsoleTitle,
            uint nSize);

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
            // Slow because testing all possible font sizes
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
            Win32ConsoleHelper.SetConsoleTitle(consoleTestName);

            byte[] receiver = new byte[consoleTestName.Length + 1];
            GetConsoleTitle(receiver, (uint)receiver.Length);

            
            Assert.AreEqual(consoleTestName, 
                // Converting the byte array to string and slicing away the null terminator
                Encoding.Default.GetString(receiver).Substring(0, receiver.Length - 1));
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
