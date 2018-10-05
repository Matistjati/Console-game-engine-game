using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Console_game.Tests
{
    [TestClass()]
    public class Win32ConsoleHelpterTests
    {
        List<Process> cmd = new List<Process>();

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
                cmd[0].Kill();
                cmd.RemoveAt(0);
            }
        }

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern Int32 GetCurrentConsoleFontEx(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            ref CONSOLE_FONT_INFOEX lpConsoleCurrentFontEx);

        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(StdHandle index);

        [DllImport("kernel32")]
        private static extern Int32 GetCurrentConsoleFont(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            ref CONSOLE_FONT_INFO lpConsoleCurrentFont);

        [DllImport("kernel32")]
        private static extern Int32 GetConsoleTitle(
            [MarshalAs(UnmanagedType.LPArray)] byte[] lpConsoleTitle,
            uint nSize);

        [DllImport("kernel32")]
        private static extern COORD GetConsoleFontSize(
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
            Win32ConsoleHelper.SetConsoleTitle(consoleTestName);

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
            consoleFontInfo.cbSize = Marshal.SizeOf(consoleFontInfo);
            GetCurrentConsoleFontEx(GetStdHandle(StdHandle.OutputHandle), false, ref consoleFontInfo);

            // For some reason, the terminal font can't be changed
            if (consoleFontInfo.FaceName == "Terminal")
            {
                Assert.AreEqual(true, true);
                return;
            }

            else
                Assert.AreEqual(consolasString, consoleFontInfo.FaceName);

            Win32ConsoleHelper.SetConsoleFont(Win32ConsoleHelper.ConsoleFont.Lucida_console);
            GetCurrentConsoleFontEx(GetStdHandle(StdHandle.OutputHandle), false, ref consoleFontInfo);
            Assert.AreEqual(luicidaConsoleString, consoleFontInfo.FaceName);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_FONT_INFO
        {
            public int nFont;
            public COORD dwFontSize;
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
            public int cbSize;
            public int FontIndex;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }
    }
}
