using System.Collections.Generic;
using System.Linq;
using static Console_game.NativeMethods;

namespace Console_game
{
    internal static class InternalInput
    {
        static bool leftMouseButtonPressed;
        static bool rightMouseButtonPressed;
        static Vector2 mousePosition = new Vector2();

        public static void MouseSetter(MOUSE_EVENT_RECORD r)
        {
            if (r.dwEventFlags == 0x0000)
            {
                leftMouseButtonPressed = r.dwButtonState == MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED ? true : false;

                rightMouseButtonPressed = r.dwButtonState == MOUSE_EVENT_RECORD.RIGHTMOST_BUTTON_PRESSED ? true : false;
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

    public enum ButtonPresses
    {
        left = 0x0001,
        right = 0x0002
    }

    public static class Input
    {
        internal static void UpdateInput()
        {
            // Copy the things being iterated

            foreach (KeyValuePair<char, bool> keyInfo in InternalInput.keysHeld)
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
        }

        private static bool leftMouseButtonPressed;
        private static bool rightMouseButtonPressed;

        public static Dictionary<char, bool> keysDown;

        public static Dictionary<char, bool> keysUp;

        public static Dictionary<char, bool> keysHeld;

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
