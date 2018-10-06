namespace Console_game
{
    static class Globals
    {
        public static readonly string gameName;

		public delegate void GameMethodSignature();

        static Globals()
        {
            gameName = "TempGameName";
        }
    }
}
