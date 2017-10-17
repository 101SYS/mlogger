using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLogger
{
    public class Configuration : ConfigurationSection
    {
        static Configuration()
        {
            Current = (Configuration)ConfigurationManager.GetSection("mLogger");
            Current.LogFileEncoding = Encoding.GetEncoding(Current.LogFileEncodingName, Encoding.UTF8.EncoderFallback, Encoding.UTF8.DecoderFallback);
        }

        public readonly static Configuration Current;


        /// <summary>
        /// Log file full path. 
        /// This setting is required in corresponding application configuration.
        /// </summary>
        [ConfigurationProperty("logFilePath", IsRequired = true)]
        public string LogFilePath
        {
            get { return (string)base["logFilePath"]; }
            protected set { base["logFilePath"] = value.Replace("$(TargetDir)", Directory.GetCurrentDirectory() + "\\"); }
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
            protected set { base["logFileMaxBytes"] = value; }
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
            protected set { base["orderEntriesByLogLevel"] = value; }
        }

        /// <summary>
        /// Log message format. 
        /// Available placeholders: 
        /// "$(LogLevel)" - replaced with message level, 
        /// "$(Message)" - replaced with message body, 
        /// "$(AdditionalInfo)" - replaced with additional information, 
        /// "$(NewLine)" - replaced with new line, 
        /// "$(TimeStamp)" - replaced with server current date-time, according to MessageTimeStampFormat. 
        /// Default: "[$(TimeStamp)|$(LogLevel)] $(Message) $(AdditionalInfo)".
        /// </summary>
        [ConfigurationProperty("messageFormat", DefaultValue = "[$(TimeStamp)|$(LogLevel)] $(Message) $(AdditionalInfo)", IsRequired = false)]
        public string MessageFormat
        {
            get { return (string)base["messageFormat"]; }
            protected set { base["messageFormat"] = value; }
        }

        /// <summary>
        /// Log message time stamp format. 
        /// DateTime format string - replaced with server current date-time. 
        /// Default: "dd/MM/yyyy HH:mm:ss.fff".
        /// </summary>
        [ConfigurationProperty("messageTimeStampFormat", DefaultValue = "dd/MM/yyyy HH:mm:ss.fff", IsRequired = false)]
        public string MessageTimeStampFormat
        {
            get { return (string)base["messageTimeStampFormat"]; }
            protected set { base["messageTimeStampFormat"] = value; }
        }


        /// <summary>
        /// Log level (severity). 
        /// Default: Debug.
        /// </summary>
        [ConfigurationProperty("logLevel", DefaultValue = LogLevel.Debug, IsRequired = false)]
        public LogLevel LogLevel
        {
            get { return (LogLevel)base["logLevel"]; }
            protected set { base["logLevel"] = value; }
        }

        /// <summary>
        /// Log file encoding name, for example "UTF-32" or "en-US". 
        /// Default: "UTF-8".
        /// </summary>
        [ConfigurationProperty("logFileEncodingName", DefaultValue = "UTF-8", IsRequired = false)]
        public string LogFileEncodingName
        {
            get { return (string)base["logFileEncodingName"]; }
            protected set
            {
                base["logFileEncodingName"] = value;
                LogFileEncoding = Encoding.GetEncoding(value, Encoding.UTF8.EncoderFallback, Encoding.UTF8.DecoderFallback);
            }
        }

        /// <summary>
        /// Log file encoding. 
        /// Default: UTF8.
        /// </summary>
        public Encoding LogFileEncoding { get; protected set; }

    }
}
