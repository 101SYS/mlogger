using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLogger.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration = MLogger.Engine.Configuration;

namespace MLogger.Engine.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void GeneralConfigurationTests()
        {
            StringAssert.Contains(Configuration.Current.LogFilePath, @"\mlog.log");
            Assert.IsFalse(Configuration.Current.LogFileMaxBytes.HasValue);
            Assert.IsFalse(Configuration.Current.IsLogFileMaxSizeLimited);
            Assert.IsFalse(Configuration.Current.OrderEntriesByLogLevel);
            Console.WriteLine("Configuration.Current.LogEncoding.EncodingName: " + Configuration.Current.LogEncoding.EncodingName);
            Assert.AreEqual<string>(Configuration.Current.LogEncoding.EncodingName, Encoding.UTF8.EncodingName);
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

            Configuration.Current.LogLevel = LogLevel.Error;
            Assert.IsTrue(Configuration.Current.IsEnabled(LogLevel.Critical)
                && Configuration.Current.IsEnabled(LogLevel.Error));
            Assert.IsFalse(Configuration.Current.IsEnabled(LogLevel.Warn)
                && Configuration.Current.IsEnabled(LogLevel.Info)
                && Configuration.Current.IsEnabled(LogLevel.Debug));
        }
    }
}