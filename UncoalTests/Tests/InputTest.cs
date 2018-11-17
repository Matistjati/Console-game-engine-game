using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uncoal.Engine;
using static Uncoal.Internal.NativeMethods;

namespace Uncoal.Tests
{
	[TestClass()]
	public class InputTest
	{
		// TODO add stress tests?

		static Process cmd;

		[ClassInitialize()]
		public static void SetUp(TestContext context)
		{
			cmd = Process.Start("cmd.exe");
			InternalInput.Start();
		}

		[ClassCleanup]
		public static void TearDown()
		{
			cmd.Kill();
		}

		static KEY_EVENT_RECORD keyEvent = new KEY_EVENT_RECORD();
		static MOUSE_EVENT_RECORD mouseEvent = new MOUSE_EVENT_RECORD();

		[TestInitialize]
		public void TestSetup()
		{
			keyEvent.AsciiChar = 0x00;
			keyEvent.bKeyDown = false;
			keyEvent.wRepeatCount = 0;
			mouseEvent.dwEventFlags = 0;
			mouseEvent.dwMousePosition.X = 0;
			mouseEvent.dwMousePosition.Y = 0;
		}

		[TestMethod()]
		public void InputKeyUpAndPressed()
		{
			keyEvent.bKeyDown = true;
			keyEvent.UnicodeChar = 'w';

			InternalInput.KeyEventHandler(keyEvent);

			Input.UpdateInput();
			Assert.IsTrue(Input.GetKeyDown('w'));

			Input.UpdateInput();
			Assert.IsTrue(Input.GetKeyUp('w'));

			Input.UpdateInput();
			Input.UpdateInput();
			Assert.AreEqual(0, Input.charsMayRelease.Count);
		}

		[TestMethod()]
		public void InputKeyHeld()
		{
			keyEvent.bKeyDown = true;
			keyEvent.wRepeatCount = 2;
			keyEvent.AsciiChar = (byte)'c';

			InternalInput.KeyEventHandler(keyEvent);
			Input.UpdateInput();

			Assert.IsTrue(Input.GetKeyHeld('c'));
		}

		[TestMethod()]
		public void InputKeyJapaneseChar()
		{
			keyEvent.bKeyDown = true;
			keyEvent.wRepeatCount = 2;
			keyEvent.UnicodeChar = '妹';

			InternalInput.KeyEventHandler(keyEvent);
			Input.UpdateInput();

			Assert.IsTrue(Input.GetKeyHeld('妹'));
		}

		[TestMethod]
		public void InputMouseLeftAndRightButtons()
		{
			mouseEvent.dwButtonState = MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED;
			InternalInput.MouseEventHandler(mouseEvent);

			Input.UpdateInput();
			Assert.IsTrue(Input.GetButtonDown(Input.ButtonPress.left));

			mouseEvent.dwButtonState = MOUSE_EVENT_RECORD.RIGHTMOST_BUTTON_PRESSED;
			InternalInput.MouseEventHandler(mouseEvent);

			Input.UpdateInput();
			Assert.IsTrue(Input.GetButtonDown(Input.ButtonPress.right));
		}

		[TestMethod]
		public void InputMousePosition()
		{
			mouseEvent.dwEventFlags = MOUSE_EVENT_RECORD.MOUSE_MOVED;
			Random rnd = new Random();

			int x = rnd.Next(0, short.MaxValue);
			int y = rnd.Next(0, short.MaxValue);

			mouseEvent.dwMousePosition.X = (short)x;
			mouseEvent.dwMousePosition.Y = (short)y;


			InternalInput.MouseEventHandler(mouseEvent);

			Input.UpdateInput();

			Assert.AreEqual(x, Input.mousePosition.X);
			Assert.AreEqual(y, Input.mousePosition.Y);
		}
	}
}
