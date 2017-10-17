using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLogger.Tests
{
    [TestClass()]
    public class MLoggerTests
    {
        const string TestMessage = "Test message: שלום Мир!";

        [TestMethod]
        public void MLoggerGeneralTests()
        {
            Console.WriteLine("MLogger.Default.LogFilePath: " + MLogger.Default.LogFilePath);
            Assert.IsNotNull(MLogger.Default.LogFilePath);
            Assert.IsTrue(!Configuration.Current.OrderEntriesByLogLevel || File.Exists(MLogger.Default.LogFilePath));
        }

        [TestMethod()]
        public void LogTest()
        {
            MLogger.Default.Log(TestMessage + " [LOG]");
            StringAssert.Contains(File.ReadAllText(MLogger.Default.LogFilePath), TestMessage);
        }


        [TestMethod]
        public void IsLogLevelEnabledTest()
        {
            Console.WriteLine("Configuration.Current.LogLevel: " + Configuration.Current.LogLevel);
            Assert.IsTrue(
                MLogger.Default.IsLogLevelEnabled(LogLevel.Critical)
                && MLogger.Default.IsLogLevelEnabled(LogLevel.Error)
                && MLogger.Default.IsLogLevelEnabled(LogLevel.Warn)
                && MLogger.Default.IsLogLevelEnabled(LogLevel.Info)
                && MLogger.Default.IsLogLevelEnabled(LogLevel.Debug));
        }

        [TestMethod()]
        public void GetLogLevelMarkerTest()
        {
            Assert.AreEqual<string>(
                Configuration.Current.OrderEntriesByLogLevel ? MLogger.LogLevelMarkerSpecialCharacter.ToString() : null,
                MLogger.Default.GetLogLevelMarker(LogLevel.Critical));
            Assert.AreEqual<string>(
                Configuration.Current.OrderEntriesByLogLevel ?
                    new string(new char[] { MLogger.LogLevelMarkerSpecialCharacter, MLogger.LogLevelMarkerSpecialCharacter }) : null,
                MLogger.Default.GetLogLevelMarker(LogLevel.Error));
        }

        [TestMethod()]
        public void GetLogLevelByMarkerTest()
        {
            LogLevel? ll = MLogger.Default.GetLogLevelByMarker("some text"); //no marker
            Assert.IsNull(ll);

            ll = MLogger.Default.GetLogLevelByMarker("some text" + MLogger.LogLevelMarkerSpecialCharacter);
            Assert.IsNotNull(ll);
            Assert.AreEqual<LogLevel>(LogLevel.Critical, ll.Value);

            ll = MLogger.Default.GetLogLevelByMarker("some text" + MLogger.LogLevelMarkerSpecialCharacter + MLogger.LogLevelMarkerSpecialCharacter);
            Assert.IsNotNull(ll);
            Assert.AreEqual<LogLevel>(LogLevel.Error, ll.Value);
        }
    }
}