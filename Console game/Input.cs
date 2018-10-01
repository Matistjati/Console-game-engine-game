using System.Collections.Generic;
using System;
using System.Linq;
using static Console_game.NativeMethods;

namespace Console_game
{
    internal static class InternalInput
    {
        public static bool leftMouseButtonPressed;
        pbulic static bool rightMouseButtonPressed;
        public static Vector2 mousePosition = new Vector2();

        public static void MouseSetter(MOUSE_EVENT_RECORD r)
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
            else if (r.dwEventFlags == 0x0001)
            {
                mousePosition.X = r.dwMousePosition.X;
                mousePosition.Y = r.dwMousePosition.Y;
            }
        }

        // Pressed
        public static Dictionary<char, bool> keysDown;

        public static Dictionary<char, bool> keysUp;

        public static Dictionary<char, bool> keysHeld;

        static readonly char[] keyStatesKeys = new char[]
            { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
                'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'å', 'ä', 'ö',
                    '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};

        static InternalInput()
        {
            keysHeld = new Dictionary<char, bool>();
            keysDown = new Dictionary<char, bool>();
            keysUp = new Dictionary<char, bool>();

            for (int i = 0; i < keyStatesKeys.Length; i++)
            {
                keysDown.Add(keyStatesKeys[i], false);
                keysHeld.Add(keyStatesKeys[i], false);
                keysUp.Add(keyStatesKeys[i], false);
            }
        }

        public static Queue<char> releasedChars = new Queue<char>();
        public static Queue<char> pressedChars = new Queue<char>();

        // TODO make this shit work
        public static void KeySetter(KEY_EVENT_RECORD r)
        {
            // Checking if the character is within what we check
            if (keyStatesKeys.Contains(r.UnicodeChar))
            {
                // Down
                if (r.bKeyDown)
                {
                    if (keysDown[r.UnicodeChar])
                    {
                        keysDown[r.UnicodeChar] = false;
                        keysHeld[r.UnicodeChar] = true;
                    }
                    else if (!keysHeld[r.UnicodeChar])
                    {
                        keysDown[r.UnicodeChar] = true;
                        pressedChars.Enqueue(r.UnicodeChar);
                    }
                }
                // Up
                else
                {
                    keysHeld[r.UnicodeChar] = false;
                    releasedChars.Enqueue(r.UnicodeChar);
                }
            }
        }
    }

    public enum ButtonPress
    {
        left = 0x0001,
        right = 0x0002
    }

    public static class Input
    {
        internal static void UpdateInput()
        {
			Dictionary<char, bool> keysHeldCopy = new Dictionary<char, bool>(InternalInput.keysHeld);

            foreach (KeyValuePair<char, bool> keyInfo in keysHeldCopy)
            {
                keysHeld[keyInfo.Key] = keyInfo.Value;
            }

            foreach (char key in keyStatesKeys)
            {
                keysUp[key] = false;
                keysDown[key] = false;
            }

            for (int i = 0; i < InternalInput.releasedChars.Count; i++)
            {
                char charChanged = InternalInput.releasedChars.Dequeue();
                keysUp[charChanged] = true;
                keysHeld[charChanged] = false;
            }

            for (int i = 0; i < InternalInput.pressedChars.Count; i++)
            {
                char charChanged = InternalInput.pressedChars.Dequeue();
                keysDown[charChanged] = true;
                keysHeld[charChanged] = false;
            }

			leftMouseButtonPressed = InternalInput.leftMouseButtonPressed;
			InternalInput.leftMouseButtonPressed = false;

			rightMouseButtonPressed = InternalInput.rightMouseButtonPressed;
			InternalInput.rightMouseButtonPressed = false;
        }

        internal static bool leftMouseButtonPressed;
        internal static bool rightMouseButtonPressed;

        internal static Dictionary<char, bool> keysDown;

        internal static Dictionary<char, bool> keysUp;

        internal static Dictionary<char, bool> keysHeld;

		public bool GetKeyDown(char key)
		{
			if (!keyStatesKeys.Contains(key)) 
			{
				throw new ArgumentException($"The char {key} is not tracked");
			}

			return keysDown[key];
		}

		public bool GetKeyHeld(char key)
		{
			if (!keyStatesKeys.Contains(key)) 
			{
				throw new ArgumentException($"The char {key} is not tracked");
			}

			return keysHeld[key];
		}

		public bool GetKeyUp(char key)
		{
			if (!keyStatesKeys.Contains(key)) 
			{
				throw new ArgumentException($"The char {key} is not tracked");
			}

			return keysUp[key];
		}

		public bool GetButtonDown(ButtonPress button)
		{
			switch (button)
			{
				case ButtonPress.left:
					return leftMouseButtonPressed;
				case ButtonPress.right:
					return rightMouseButtonPressed;
				default:
					Globals.logger.LogException($"Case default was reached: {button}");
					return false;
			}

		}
		
        static readonly char[] keyStatesKeys = new char[]
            { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
                'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'å', 'ä', 'ö',
                    '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};

        static Input()
        {
            keysHeld = new Dictionary<char, bool>();
            keysDown = new Dictionary<char, bool>();
            keysUp = new Dictionary<char, bool>();

            for (int i = 0; i < keyStatesKeys.Length; i++)
            {
                keysDown.Add(keyStatesKeys[i], false);
                keysHeld.Add(keyStatesKeys[i], false);
                keysUp.Add(keyStatesKeys[i], false);
            }
        }
    }
}
