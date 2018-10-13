using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using static Console_game.NativeMethods;

namespace Console_game.Tests
{
    [TestClass]
    public class ConsoleListenerTests
    {
        static Process cmd;

        [ClassInitialize()]
        public static void SetUp(TestContext context)
        {
            cmd = Process.Start("cmd.exe");
        }

        [ClassCleanup]
        public static void TearDown()
        {
            cmd.Kill();
        }

        static readonly Random randomGen = new Random();

        [TestMethod]
        public void ConsoleListenerMouseEventTest()
        {
            // If this method fails, try setting the sleep to something higher

            short xCoord = (short)randomGen.Next(0, short.MaxValue);
            short yCoord = (short)randomGen.Next(0, short.MaxValue);
            uint buttonPressed = (randomGen.Next(0, 2) == 1) ?
                (uint)MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED :
                (uint)MOUSE_EVENT_RECORD.RIGHTMOST_BUTTON_PRESSED;


            INPUT_RECORD[] record = new INPUT_RECORD[1];
            record[0] = new INPUT_RECORD
            {
                EventType = INPUT_RECORD.MOUSE_EVENT,
                MouseEvent = new MOUSE_EVENT_RECORD
                {
                    dwMousePosition = new COORD(xCoord, yCoord),
                    dwButtonState = buttonPressed
                }
            };

            ConsoleListener.Start();
            ConsoleListener.MouseEvent += ButtonDown;

            uint recordsWritten = 0;
            WriteConsoleInput(GetStdHandle(StdHandle.InputHandle), record, 1, ref recordsWritten);

            // Giving time for buttondown to be called
            Thread.Sleep(1);

            Assert.IsTrue(buttonDownCalled);
            Assert.AreEqual(xCoord, mouseEvent.dwMousePosition.X);
            Assert.AreEqual(yCoord, mouseEvent.dwMousePosition.Y);
            Assert.AreEqual(buttonPressed, mouseEvent.dwButtonState);
        }

        MOUSE_EVENT_RECORD mouseEvent;
        bool buttonDownCalled = false;
        private void ButtonDown(MOUSE_EVENT_RECORD m)
        {
            buttonDownCalled = true;
            mouseEvent = m;
        }

        [TestMethod]
        public void ConsoleListenerKeyEventTest()
        {
            // If this method fails, try setting the sleep to something higher

            byte keyPressed = (byte)randomGen.Next(97, 122);
            bool keyDown = (randomGen.Next(0, 2) == 1) ? true : false;
            ushort repeatCount = (ushort)randomGen.Next(1, 6);

            INPUT_RECORD[] record = new INPUT_RECORD[1];
            record[0] = new INPUT_RECORD
            {
                EventType = INPUT_RECORD.KEY_EVENT,
                KeyEvent = new KEY_EVENT_RECORD()
                {
                    AsciiChar = keyPressed,
                    UnicodeChar = (char)keyPressed,
                    bKeyDown = keyDown,
                    wRepeatCount = repeatCount
                }
            };


            ConsoleListener.Start();
            ConsoleListener.KeyEvent += KeyDown;

            uint recordsWritten = 0;
            WriteConsoleInput(GetStdHandle(StdHandle.InputHandle), record, 1, ref recordsWritten);

            // Giving time for keydown to be called
            Thread.Sleep(1);

            Assert.IsTrue(keyDownCalled);
            Assert.AreEqual(keyPressed, keyDownResult.AsciiChar);
            Assert.AreEqual(keyPressed, (byte)keyDownResult.UnicodeChar);
            Assert.AreEqual(keyDown, keyDownResult.bKeyDown);
            Assert.AreEqual(repeatCount, keyDownResult.wRepeatCount);
        }

        KEY_EVENT_RECORD keyDownResult;
        bool keyDownCalled = false;
        private void KeyDown(KEY_EVENT_RECORD k)
        {
            keyDownCalled = true;
            keyDownResult = k;
        }

        [TestMethod]
        public void ConsoleListenerBufferSizeEventTest()
        {
            // If this method fails, try setting the sleep to something higher

            short xCoord = (short)randomGen.Next(0, short.MaxValue);
            short yCoord = (short)randomGen.Next(0, short.MaxValue);

            INPUT_RECORD[] record = new INPUT_RECORD[1];
            record[0] = new INPUT_RECORD
            {
                EventType = INPUT_RECORD.WINDOW_BUFFER_SIZE_EVENT,
                WindowBufferSizeEvent = new WINDOW_BUFFER_SIZE_RECORD()
                {
                    dwSize = new COORD(xCoord, yCoord)
                }
            };


            ConsoleListener.Start();
            ConsoleListener.WindowBufferSizeEvent += BufferSizeChanged;

            uint recordsWritten = 0;
            WriteConsoleInput(GetStdHandle(StdHandle.InputHandle), record, 1, ref recordsWritten);

            // Giving time for keydown to be called
            Thread.Sleep(1);

            Assert.IsTrue(bufferSizeChangedCalled);
            Assert.AreEqual(xCoord, actualBufferSize.X);
            Assert.AreEqual(yCoord, actualBufferSize.Y);
        }

        COORD actualBufferSize;
        bool bufferSizeChangedCalled = false;
        private void BufferSizeChanged(WINDOW_BUFFER_SIZE_RECORD w)
        {
            actualBufferSize = w.dwSize;
            bufferSizeChangedCalled = true;
        }
    }
}
