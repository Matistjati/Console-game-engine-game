using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Console_game
{
    internal class Log
    {
        public string filePath;

        public bool showDate;
        public bool showLineNumber;
        public bool showCaller;
        public bool showFileName;

        private FileStream logStream;

        public Log(string filePath, bool showDate, bool showLineNumber, bool showCaller, bool showFileName)
        {
            if (!File.Exists(filePath))
            {
                logStream = File.Create(filePath);
                logStream.Close();
            }

            this.filePath = filePath;
            this.showDate = showDate;
            this.showLineNumber = showLineNumber;
            this.showCaller = showCaller;
            this.showFileName = showFileName;
        }

        public void logInfo(string info,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string callerFile = null)
        {
            string formattedString = formatString(info, lineNumber, caller, callerFile);
            File.AppendAllText(filePath, formattedString);
        }

        private string formatString(string logString,
            int lineNumber,
            string caller,
            string callerFile)
        {

            if (showLineNumber)
            {
                logString = $"at line {lineNumber} " + logString;
            }
            if (showCaller)
            {
                logString = $"called by {caller} " + logString;
            }
            if (showFileName)
            {
                logString = $"at file {Path.GetFileName(callerFile)} " + logString;
            }
            if (showDate)
            {
                logString = DateTime.Now.ToString("yyyy/MM/dd, HH:mm ") + logString;
            }

            logString += Environment.NewLine;

            return logString;
        }
    }
}
