using System.IO;

namespace Console_game
{
    static class Globals
    {
        public static Log logger;
        public static readonly string gameName;

        static Globals()
        {
            gameName = "TempGameName";

            // Creating the necessary folders and files
            Directory.CreateDirectory("logs");
            using (StreamWriter x = File.AppendText("logs/log.txt"))

            // Creating the logger with all modes set to true
            logger = new Log("logs/log.txt",
                showDate: true,
                showLineNumber: true,
                showCaller: true,
                showFileName: true);
        }
    }
}
