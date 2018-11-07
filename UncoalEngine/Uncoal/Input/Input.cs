using System.Collections.Generic;
using Uncoal.Internal;
using static Uncoal.Internal.NativeMethods;

namespace Uncoal.Engine
{
	internal static class InternalInput
	{
		public static void Start()
		{
			ConsoleListener.MouseEvent += MouseEventHandler;
			ConsoleListener.KeyEvent += KeyEventHandler;
			ConsoleListener.Start();
		}

		public static void Stop()
		{
			ConsoleListener.Stop();
		}

		public static void Reset()
		{
			leftMouseButtonPressed = false;
			rightMouseButtonPressed = false;
			mousePosition.Set(0, 0);

			pressedChars.Clear();
			heldChars.Clear();
			Input.charsMayRelease.Clear();
			Input.heldChars.Clear();
			Input.releasedChars.Clear();
			Input.pressedChars.Clear();
		}

		public static bool leftMouseButtonPressed;
		public static bool rightMouseButtonPressed;
		public static Coord mousePosition = new Coord();

		public static void MouseEventHandler(MOUSE_EVENT_RECORD r)
		{
			if (r.dwEventFlags == 0x0000)
			{
				if (r.dwButtonState == MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED)
				{
					leftMouseButtonPressed = true;
				}
				else if (r.dwButtonState == MOUSE_EVENT_RECORD.RIGHTMOST_BUTTON_PRESSED)
				{
					rightMouseButtonPressed = true;
				}
			}
			else if (r.dwEventFlags == MOUSE_EVENT_RECORD.MOUSE_MOVED)
			{
				//mousePosition.Set((uint)r.dwMousePosition.X, (uint)r.dwMousePosition.Y);
				mousePosition.Set((uint)r.dwMousePosition.X, (uint)r.dwMousePosition.Y);
			}
		}

		public static Queue<char> pressedChars = new Queue<char>();
		public static Queue<char> heldChars = new Queue<char>();

		public static void KeyEventHandler(KEY_EVENT_RECORD r)
		{
			if (r.bKeyDown)
			{
				if (r.wRepeatCount > 1 && !pressedChars.Contains(r.UnicodeChar))
				{
					heldChars.Enqueue(r.UnicodeChar);
				}
				else
				{
					pressedChars.Enqueue(r.UnicodeChar);
				}
			}
		}


	}

	public static class Input
	{
		internal static void Reset() => InternalInput.Reset();

		internal static Queue<char> charsMayRelease = new Queue<char>();

		internal static HashSet<char> heldChars = new HashSet<char>();
		internal static HashSet<char> releasedChars = new HashSet<char>();
		internal static HashSet<char> pressedChars = new HashSet<char>();

		internal static void UpdateInput()
		{
			heldChars.Clear();
			releasedChars.Clear();
			pressedChars.Clear();

			for (int i = 0; i < charsMayRelease.Count; i++)
			{
				char potentiallyReleased = charsMayRelease.Dequeue();
				if (!InternalInput.heldChars.Contains(potentiallyReleased) &&
					!InternalInput.pressedChars.Contains(potentiallyReleased))
				{
					releasedChars.Add(potentiallyReleased);
				}
				else if (!charsMayRelease.Contains(potentiallyReleased))
				{
					charsMayRelease.Enqueue(potentiallyReleased);
				}
			}

			for (int i = 0; i < InternalInput.pressedChars.Count; i++)
			{
				char charChanged = InternalInput.pressedChars.Dequeue();
				pressedChars.Add(charChanged);
				charsMayRelease.Enqueue(charChanged);
			}

			for (int i = 0; i < InternalInput.heldChars.Count; i++)
			{
				char charChanged = InternalInput.heldChars.Dequeue();
				heldChars.Add(charChanged);
				charsMayRelease.Enqueue(charChanged);
			}

			leftMouseButtonPressed = InternalInput.leftMouseButtonPressed;
			InternalInput.leftMouseButtonPressed = false;

			rightMouseButtonPressed = InternalInput.rightMouseButtonPressed;
			InternalInput.rightMouseButtonPressed = false;

			mousePosition.Set(InternalInput.mousePosition.X, InternalInput.mousePosition.Y);
		}

		public static bool GetKeyDown(char key) => pressedChars.Contains(key);

		public static bool GetKeyHeld(char key) => heldChars.Contains(key);

		public static bool GetKeyUp(char key) => releasedChars.Contains(key);

		public static Coord mousePosition = new Coord();

		public enum ButtonPress
		{
			left = 0x0001,
			right = 0x0002
		}

		internal static bool leftMouseButtonPressed;
		internal static bool rightMouseButtonPressed;

		public static bool GetButtonDown(ButtonPress button)
		{
			switch (button)
			{
				case ButtonPress.left:
					return leftMouseButtonPressed;
				case ButtonPress.right:
					return rightMouseButtonPressed;
				default:
					Log.DefaultLogger.LogError($"Case default was reached: {button}");
					return false;
			}
		}
	}
}
