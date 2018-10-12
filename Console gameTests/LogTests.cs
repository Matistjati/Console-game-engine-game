using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Console_game.Tests
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
        public void LogTestConstructorFileExists()
        {
            const string loggedString = "hai";
            Log log = new Log("log.txt", true, true, true, true);

            StackFrame currentState = new StackFrame(true);

            log.LogInfo(loggedString);

            int lineNumber = currentState.GetFileLineNumber();
            lineNumber++; // The log calling is one line under getting the line number

            // It's beautiful and you can't change my mind
            string cleanMethodName = currentState.GetMethod().ToString().Substring(0, currentState.GetMethod().ToString().Length - 2).Substring((int)(currentState.GetMethod() as MethodInfo).ReturnParameter.ParameterType.ToString().Substring("System.".Length).Length + 1);

            string formattedLogString = $"{DateTime.Now.ToString("yyyy/MM/dd, HH:mm")} at file" +
                $" {Path.GetFileName(currentState.GetFileName())} called by {cleanMethodName}" +
                $" at line {lineNumber} Info: {loggedString}";

            Assert.AreEqual(
                formattedLogString,
                File.ReadAllText(logName).Trim());
        }
    }
}
