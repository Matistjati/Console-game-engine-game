using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uncoal.Internal;

namespace Uncoal.Tests
{
	[TestClass]
	public class LogTests
	{
		private const string logName = "log.txt";

		[ClassInitialize]
		public static void SetUp(TestContext t)
		{
			if (!File.Exists(logName))
				using (File.Create(logName)) { }
		}

		[ClassCleanup]
		public static void TearDown()
		{
			File.Delete(logName);
		}

		[TestCleanup]
		public void MethodTearDown()
		{
			File.WriteAllText(logName, string.Empty);
		}

		[TestMethod]
		public void LogConstructorFileExistsTest()
		{
			Log log = new Log(logName, true, true, true, true);
		}

		[TestMethod]
		public void LogConstructorFileDoesntExistsTest()
		{
			Log log = new Log("Sample.txt", true, true, true, true);
			Assert.IsTrue(File.Exists("Sample.txt"));
			File.Delete("Sample.txt");
		}

		[TestMethod]
		public void LogConstructorFileTreeDoesntExistsTest()
		{
			Log log = new Log("folding/folder/text.txt", true, true, true, true);
			Assert.IsTrue(File.Exists("folding/folder/text.txt"));
			DirectoryInfo directory = new DirectoryInfo("folding");
			directory.Delete(true);
		}

		private enum LogLevel
		{
			Info, Warning, Error
		}

		private string FormatString(string text, StackFrame currentState, bool line, bool date, bool method, bool fileName, LogLevel logLevel)
		{
			int lineNumber = currentState.GetFileLineNumber();
			lineNumber++; // The log calling is one line under getting the line number

			// It's beautiful and you can't change my mind
			string cleanMethodName = "called by " + currentState.GetMethod().ToString().Substring(0, currentState.GetMethod().ToString().Length - 2).Substring((currentState.GetMethod() as MethodInfo).ReturnParameter.ParameterType.ToString().Substring("System.".Length).Length + 1) + " ";

			string dateString = $"{DateTime.Now.ToString("yyyy/MM/dd, HH:mm")}" + " ";
			string fileString = "at file " + Path.GetFileName(currentState.GetFileName()) + " ";
			string lineString = "at line " + lineNumber + " ";

			return $"{(date ? dateString : string.Empty)}" +
				$"{(fileName ? fileString : string.Empty)}{(method ? cleanMethodName : string.Empty)}" +
				$"{(line ? lineString : string.Empty)}{logLevel}: {text}";
		}

		Log testLogger = new Log(logName, true, true, true, true);
		Random random = new Random();


		[TestMethod]
		public void Log_LogInfoTest()
		{
			const string loggedString = "hai";

			// Setting the log flags randomly
			bool showMethod = (random.Next(0, 2) == 1) ? true : false;
			bool showDate = (random.Next(0, 2) == 1) ? true : false;
			bool showLine = (random.Next(0, 2) == 1) ? true : false;
			bool showFile = (random.Next(0, 2) == 1) ? true : false;

			testLogger.showCaller = showMethod;
			testLogger.showDate = showDate;
			testLogger.showLineNumber = showLine;
			testLogger.showFileName = showFile;

			StackFrame currentState = new StackFrame(true);
			testLogger.LogInfo(loggedString);
			string formattedText = FormatString(loggedString, currentState, showLine, showDate, showMethod, showFile, LogLevel.Info);

			Assert.AreEqual(
				formattedText,
				File.ReadAllText(logName).Trim());
		}

		[TestMethod]
		public void Log_LogWarningTest()
		{
			const string loggedString = "hoiiii";
			bool showMethod = (random.Next(0, 2) == 1) ? true : false;
			bool showDate = (random.Next(0, 2) == 1) ? true : false;
			bool showLine = (random.Next(0, 2) == 1) ? true : false;
			bool showFile = (random.Next(0, 2) == 1) ? true : false;

			testLogger.showCaller = showMethod;
			testLogger.showDate = showDate;
			testLogger.showLineNumber = showLine;
			testLogger.showFileName = showFile;

			StackFrame currentState = new StackFrame(true);
			testLogger.LogWarning(loggedString);
			string formattedText = FormatString(loggedString, currentState, showLine, showDate, showMethod, showFile, LogLevel.Warning);

			Assert.AreEqual(
				formattedText,
				File.ReadAllText(logName).Trim());
		}

		[TestMethod]
		public void Log_LogErrorTest()
		{
			const string loggedString = "I Rise from the void bringing forth this unreadable piece of code";
			bool showMethod = (random.Next(0, 2) == 1) ? true : false;
			bool showDate = (random.Next(0, 2) == 1) ? true : false;
			bool showLine = (random.Next(0, 2) == 1) ? true : false;
			bool showFile = (random.Next(0, 2) == 1) ? true : false;

			testLogger.showCaller = showMethod;
			testLogger.showDate = showDate;
			testLogger.showLineNumber = showLine;
			testLogger.showFileName = showFile;

			StackFrame currentState = new StackFrame(true);
			testLogger.LogError(loggedString);
			string formattedText = FormatString(loggedString, currentState, showLine, showDate, showMethod, showFile, LogLevel.Error);

			Assert.AreEqual(
				formattedText,
				File.ReadAllText(logName).Trim());
		}
	}
}
