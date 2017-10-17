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
        public void ConfigGeneralTests()
        {
            Console.WriteLine(Path.GetDirectoryName(Configuration.Current.LogFilePath));
            StringAssert.Contains(Configuration.Current.LogFilePath, @"\mlog.log");
            Assert.IsFalse(Configuration.Current.LogFileMaxBytes.HasValue);
            Assert.IsFalse(Configuration.Current.IsLogFileMaxSizeLimited);
            Assert.IsNotNull(Configuration.Current.MessageFormat);
            Assert.IsNotNull(Configuration.Current.MessageTimeStampFormat);
            Console.WriteLine("Configuration.Current.LogEncoding.EncodingName: " + Configuration.Current.LogFileEncoding.EncodingName);
            Assert.AreEqual<string>(Configuration.Current.LogFileEncoding.EncodingName, Encoding.UTF8.EncodingName);
        }

    }
}