using System;

namespace Console_game
{
    class GameObject
    {
		internal static float _timeDelta;
		internal static float _time;

        public static float timeDelta
		{
			get
			{
				return _timeDelta;
			}
		}
        public static float time
		{
			get
			{
				return _time;
			}
		}
    }
}
