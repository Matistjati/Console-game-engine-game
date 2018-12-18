using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
			mousePosition.X = 0;
			mousePosition.Y = 0;

			pressedChars.Clear();
			heldChars.Clear();
			Input.heldChars.Clear();
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
				mousePosition.X = r.dwMousePosition.X;
				mousePosition.Y = r.dwMousePosition.Y;
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

		internal static HashSet<char> heldChars = new HashSet<char>();
		internal static HashSet<char> pressedChars = new HashSet<char>();

		internal static void UpdateInput()
		{
			heldChars.Clear();
			pressedChars.Clear();


			for (int i = 0; i < InternalInput.pressedChars.Count; i++)
			{
				pressedChars.Add(InternalInput.pressedChars.Dequeue());
			}

			for (int i = 0; i < InternalInput.heldChars.Count; i++)
			{
				heldChars.Add(InternalInput.heldChars.Dequeue());
			}

			leftMouseButtonPressed = InternalInput.leftMouseButtonPressed;
			InternalInput.leftMouseButtonPressed = false;

			rightMouseButtonPressed = InternalInput.rightMouseButtonPressed;
			InternalInput.rightMouseButtonPressed = false;

			mousePosition.X = InternalInput.mousePosition.X;
			mousePosition.Y = InternalInput.mousePosition.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetKeyDown(char key) => pressedChars.Contains(key);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetKeyHeld(char key) => heldChars.Contains(key);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetKeyUp(char key) => !pressedChars.Contains(key) && !heldChars.Contains(key);

		public static Coord mousePosition = new Coord();

		public static bool leftMouseButtonPressed;
		public static bool rightMouseButtonPressed;
	}
}
