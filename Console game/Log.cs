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
  
        private static Log _defaultLogger;
        
		internal static Log DefaultLogger
		{
			get
			{
				if (_defaultLogger == null)
				{
					_defaultLogger = new Log("logs/log.txt",
						showDate: true,
						showLineNumber: true,
						showCaller: true,
						showFileName: true);
				}
                return _defaultLogger;
			}
		}
        
        public Log(string logPath, bool showDate, bool showLineNumber, bool showCaller, bool showFileName)
        {
            try
            {
                if (!File.Exists(filePath))
                    using (File.Create(logPath)) { }
            }
            catch (DirectoryNotFoundException)
            {
                string logName = Path.GetFileName(logPath);
                string logPathNoFile = Path.GetDirectoryName(logPath);
                Directory.CreateDirectory(logPathNoFile);
                using (File.Create(Path.Combine(logPathNoFile, logName))) { }
            }

            this.filePath = logPath;
            this.showDate = showDate;
            this.showLineNumber = showLineNumber;
            this.showCaller = showCaller;
            this.showFileName = showFileName;
        }
            
        public void LogInfo<T>(T info,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string callerFile = null)
        {
            log(info.ToString(), lineNumber, caller, callerFile, LogLevel.Info);
        }

        public void LogWarning<T>(T info,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string callerFile = null)
        {
            log(info.ToString(), lineNumber, caller, callerFile, LogLevel.Warning);
        }

        public void LogError<T>(T info,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string callerFile = null)
        {
            log(info.ToString(), lineNumber, caller, callerFile, LogLevel.Error);
        }

        private enum LogLevel
        {
            Info, Warning, Error
        }

        private void log(string logText, int lineNumber, string caller, string callerFile, LogLevel logLevel)
        {
            string formattedString = FormatString(logText, lineNumber, caller, callerFile, logLevel);
            File.AppendAllText(filePath, formattedString);
        }

        private string FormatString(string logString,
            int lineNumber,
            string caller,
            string callerFile,
            LogLevel logLevel)
        {
            logString = $"{logLevel}: {logString}";

            if (showLineNumber)
            {
                logString = $"at line {lineNumber} {logString}";
            }
            if (showCaller)
            {
                logString = $"called by {caller} {logString}";
            }
            if (showFileName)
            {
                logString = $"at file {Path.GetFileName(callerFile)} {logString}";
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
