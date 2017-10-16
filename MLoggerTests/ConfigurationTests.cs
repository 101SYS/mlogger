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
            Assert.IsFalse(Configuration.Current.OrderEntriesByLogLevel);
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
    }
}