namespace Console_game
{
    internal static class Globals
    {
        public static readonly string gameName;

		public delegate void GameMethodSignature();

        static Globals()
        {
            gameName = "TempGameName";
        }
    }
}
