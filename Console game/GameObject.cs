using System.Drawing;

namespace Console_game
{
    public class GameObject
    {
        public Coord Position { get; set; } = new Coord();

        internal static float _timeDelta;
		internal static float _time;

        public static float TimeDelta => _timeDelta;

        public static float Time => _time;
    }
}
