using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration = MLogger.Configuration;

namespace MLogger.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void GeneralConfigurationTests()
        {
            Console.WriteLine(Path.GetDirectoryName(Configuration.Current.LogFilePath));
            StringAssert.Contains(Configuration.Current.LogFilePath, @"\mlog.log");
            Assert.IsFalse(Configuration.Current.LogFileMaxBytes.HasValue);
            Assert.IsFalse(Configuration.Current.IsLogFileMaxSizeLimited);
            Assert.IsTrue(Configuration.Current.OrderEntriesByLogLevel);
            Assert.IsNotNull(Configuration.Current.LogEntryFormat);
            Console.WriteLine("Configuration.Current.LogEncoding.EncodingName: " + Configuration.Current.LogFileEncoding.EncodingName);
            Assert.AreEqual<string>(Configuration.Current.LogFileEncoding.EncodingName, Encoding.UTF8.EncodingName);
        }

        [TestMethod]
        public void IsEnabledTest()
        {
            Console.WriteLine("Configuration.Current.LogLevel: " + Configuration.Current.LogLevel);
            Assert.IsTrue(
                Configuration.Current.IsEnabled(LogLevel.Critical)
                && Configuration.Current.IsEnabled(LogLevel.Error)
                && Configuration.Current.IsEnabled(LogLevel.Warn)
                && Configuration.Current.IsEnabled(LogLevel.Info)
                && Configuration.Current.IsEnabled(LogLevel.Debug));

            //Configuration.Current.LogLevel = LogLevel.Error;
            //Assert.IsTrue(Configuration.Current.IsEnabled(LogLevel.Critical)
            //    && Configuration.Current.IsEnabled(LogLevel.Error));
            //Assert.IsFalse(Configuration.Current.IsEnabled(LogLevel.Warn)
            //    && Configuration.Current.IsEnabled(LogLevel.Info)
            //    && Configuration.Current.IsEnabled(LogLevel.Debug));
        }

        [TestMethod()]
        public void GetLogLevelMarkerTest()
        {
            Assert.AreEqual<string>(Configuration.Current.GetLogLevelMarker(LogLevel.Critical), Configuration.LogLevelMarkerSpecialCharacter.ToString());
            Assert.AreEqual<string>(Configuration.Current.GetLogLevelMarker(LogLevel.Error),
                new string(new char[] { Configuration.LogLevelMarkerSpecialCharacter, Configuration.LogLevelMarkerSpecialCharacter }));
        }

        [TestMethod()]
        public void GetLogLevelByMarkerTest()
        {
            LogLevel? ll = Configuration.Current.GetLogLevelByMarker("some text" + Configuration.LogLevelMarkerSpecialCharacter);
            Assert.IsNotNull(ll);
            Assert.AreEqual<LogLevel>(LogLevel.Critical, ll.Value);

            ll = Configuration.Current.GetLogLevelByMarker("some text" + Configuration.LogLevelMarkerSpecialCharacter + Configuration.LogLevelMarkerSpecialCharacter);
            Assert.IsNotNull(ll);
            Assert.AreEqual<LogLevel>(LogLevel.Error, ll.Value);
        }
    }
}