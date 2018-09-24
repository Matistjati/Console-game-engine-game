using System.IO;

namespace Console_game
{
    static class Globals
    {
        public static Log logger;

        static Globals()
        {
            // Creating the necessary folders and files
            Directory.CreateDirectory("logs");
            using (StreamWriter x = File.AppendText("logs/log.txt"))

            // Creating the logger with all modes set to true
            logger = new Log("logs/log.txt",
                showDate: true,
                showLineNumber: true,
                showCaller: true);
        }
    }
}
