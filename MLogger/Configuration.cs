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
        public Configuration()
        {
            LogFilePath = LogFilePath;
            LogFileEncodingName = LogFileEncodingName;
            LogLevelMarkers = Enum.GetValues(typeof(LogLevel)).Cast<byte>().OrderBy(value => value)
                .Select(value => new string(Enumerable.Repeat(LogLevelMarkerSpecialCharacter, value + 1).ToArray()))
                .ToList().AsReadOnly();
        }

        static Configuration()
        {
            Current = new Configuration();
        }

        public readonly static Configuration Current;


        /// <summary>
        /// Log file full path. 
        /// Available placeholders: "$(TargetDir)" - replaced with current executing location. 
        /// Default: "$(TargetDir)mlog.log".
        /// </summary>
        [ConfigurationProperty("logFilePath", DefaultValue = "$(TargetDir)mlog.log", IsRequired = true)]
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
        [ConfigurationProperty("orderEntriesByLogLevel", DefaultValue = true, IsRequired = false)]
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
        /// DateTime format string - replaced with server current date-time. 
        /// Default: "[dd/MM/yyyy HH:mm:ss.fff|$(LogLevel)] $(Message) $(AdditionalInfo)".
        /// </summary>
        [ConfigurationProperty("logEntryFormat", DefaultValue = "[dd/MM/yyyy HH:mm:ss.fff|$(LogLevel)] $(Message) $(AdditionalInfo)", IsRequired = true)]
        public string LogEntryFormat
        {
            get { return (string)base["logEntryFormat"]; }
            protected set { base["logEntryFormat"] = value; }
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
        /// Verifies if specified log level enabled.
        /// </summary>
        /// <param name="logLevel">Log level to verify</param>
        /// <returns>true or false</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return this.LogLevel >= logLevel;
        }

        public const char LogLevelMarkerSpecialCharacter = (char)255;
        private IReadOnlyList<string> LogLevelMarkers { get; set; }
        /// <summary>
        /// Returns log level marker by provided log level. 
        /// If OrderEntriesByLogLevel is true - should be added to message to specify its log level.
        /// </summary>
        public string GetLogLevelMarker(LogLevel logLevel)
        {
            return OrderEntriesByLogLevel ? LogLevelMarkers[((int)logLevel)] : null;
        }
        /// <summary>
        /// Returns log level by provided marker or null if marker not available.
        /// </summary>
        public LogLevel? GetLogLevelByMarker(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            int specialCharactersCount = -1;
            int position = text.Length - 1;
            while(text[position--] == LogLevelMarkerSpecialCharacter)
            {
                ++specialCharactersCount;
            }
            return specialCharactersCount < 0 ? (LogLevel?)null : (LogLevel?)specialCharactersCount;
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
