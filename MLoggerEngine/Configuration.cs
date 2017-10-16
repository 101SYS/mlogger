using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLogger.Engine
{
    public sealed class Configuration : ConfigurationSection
    {
        public Configuration()
        {
            LogFilePath = LogFilePath;
            EncodingName = EncodingName;
        }

        static Configuration()
        {
            Current = new Configuration();
        }

        public readonly static Configuration Current;


        /// <summary>
        /// Log file full path. 
        /// Available macros "$(TargetDir)" - replaced with current executing location. 
        /// Default: "$(TargetDir)mlog.log".
        /// </summary>
        [ConfigurationProperty("logFilePath", DefaultValue = "$(TargetDir)mlog.log", IsRequired = true)]
        public string LogFilePath
        {
            get { return (string)base["logFilePath"]; }
            set { base["logFilePath"] = value.Replace("$(TargetDir)", Directory.GetCurrentDirectory() + "\\"); }
        }


        /// <summary>
        /// Log file max size in bytes limitation. 
        /// When a file reaches defined size, it is renamed and a new file is started for the current logs. 
        /// null, negative or zero values means that the file size is unlimited. 
        /// Default: null (unlimited).
        /// </summary>
        [ConfigurationProperty("logFileMaxBytes", DefaultValue = null, IsRequired = false)]
        public long? LogFileMaxBytes
        {
            get { return (long?)base["logFileMaxBytes"]; }
            set { base["logFileMaxBytes"] = value; }
        }

        /// <summary>
        /// Is log file max size limited. 
        /// True when LogFileMaxBytes has positive, grater than 0 value.
        /// </summary>
        public bool IsLogFileMaxSizeLimited
        {
            get { return LogFileMaxBytes.HasValue && LogFileMaxBytes.Value > 0L; }
        }

        /// <summary>
        /// Defines, if log entries should be sorted (grouped) by log level (severity) or not. 
        /// Default: false.
        /// </summary>
        [ConfigurationProperty("orderEntriesByLogLevel", DefaultValue = false, IsRequired = false)]
        public bool OrderEntriesByLogLevel
        {
            get { return (bool)base["orderEntriesByLogLevel"]; }
            set { base["orderEntriesByLogLevel"] = value; }
        }


        /// <summary>
        /// Log level (severity). 
        /// Default: Debug.
        /// </summary>
        [ConfigurationProperty("logLevel", DefaultValue = LogLevel.Debug, IsRequired = false)]
        public LogLevel LogLevel
        {
            get { return (LogLevel)base["logLevel"]; }
            set { base["logLevel"] = value; }
        }

        /// <summary>
        /// Verifies if specified log level enabled.
        /// </summary>
        /// <param name="logLevel">Log level to verify</param>
        /// <returns>true or false</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return this.LogLevel >= logLevel;
        }

        /// <summary>
        /// Log encoding, for example "utf-32" or "en-US". 
        /// Default: "UTF-8".
        /// </summary>
        [ConfigurationProperty("encodingName", DefaultValue = "UTF-8", IsRequired = false)]
        public string EncodingName
        {
            get { return (string)base["encodingName"]; }
            set
            {
                base["encodingName"] = value;
                LogEncoding = Encoding.GetEncoding(value, Encoding.UTF8.EncoderFallback, Encoding.UTF8.DecoderFallback);
            }
        }

        /// <summary>
        /// Log file encoding. 
        /// Default: UTF8.
        /// </summary>
        public Encoding LogEncoding { get; set; }

    }
}
