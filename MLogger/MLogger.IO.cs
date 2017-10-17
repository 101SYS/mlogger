using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLogger
{
    public partial class MLogger
    {
        /// <summary>
        /// Gets new log file stream reader with encoding. 
        /// If log file does not exists - creates new log file.
        /// </summary>
        protected StreamReader NewReader
        {
            get
            {
                return new StreamReader(
                File.Open(this.LogFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite),
                Configuration.Current.LogFileEncoding, false);
            }
        }

        /// <summary>
        /// Gets new log file stream writer with encoding. 
        /// If log file does not exists - creates new log file.
        /// </summary>
        protected StreamWriter NewWriter
        {
            get
            {
                return new StreamWriter(
                File.Open(this.LogFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read),
                Configuration.Current.LogFileEncoding);
            }
        }

        /// <summary>
        /// Gets new log file stream writer with encoding in append mode (seek to the end of the file). 
        /// If log file does not exists - creates new log file.
        /// </summary>
        protected StreamWriter NewAppender
        {
            get
            {
                return new StreamWriter(
                File.Open(this.LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read),
                Configuration.Current.LogFileEncoding);
            }
        }

        /// <summary>
        /// Updates changed log-level blocks positions, accordingly to saved text
        /// </summary>
        /// <param name="text">saved text</param>
        /// <returns>LogLevel or, if text contains no log-level marks - returns null</returns>
        protected LogLevel? UpdatePositions(string text)
        {
            LogLevel? logLevel = GetLogLevelByMarker(text);
            UpdatePositions(logLevel, text);
            return logLevel;
        }

        /// <summary>
        /// Updates changed log-level blocks positions, accordingly to saved text log-level
        /// </summary>
        /// <param name="logLevel">saved text log-level</param>
        /// <param name="text">saved text</param>
        protected void UpdatePositions(LogLevel? logLevel, string text)
        {
            int textLength = Configuration.Current.LogFileEncoding.GetByteCount(text);
            int logLevelIndex = logLevel.HasValue ? (int)logLevel.Value : 0;
            for (int i = Positions.Count; i >= logLevelIndex; i--)
            {
                Positions[i] += textLength;
            }
        }

        /// <summary>
        /// Saves message sequentially (appends it to the end of the file)
        /// </summary>
        /// <param name="message">message text</param>
        protected void SaveMessageSequentially(string message)
        {
            using (var sw = NewAppender)
            {
                sw.WriteLine(message);
            }
        }

        /// <summary>
        /// Verifies if specified log level enabled.
        /// </summary>
        /// <param name="logLevel">Log level to verify</param>
        /// <returns>true or false</returns>
        public bool IsLogLevelEnabled(LogLevel logLevel)
        {
            return Configuration.Current.LogLevel >= logLevel;
        }

        public const char LogLevelMarkerSpecialCharacter = (char)0;
        private IReadOnlyList<string> LogLevelMarkers { get; set; }
        /// <summary>
        /// Returns log level marker by provided log level. 
        /// If OrderEntriesByLogLevel is true - should be added to message to specify its log level.
        /// </summary>
        public string GetLogLevelMarker(LogLevel logLevel)
        {
            return Configuration.Current.OrderEntriesByLogLevel ? LogLevelMarkers[((int)logLevel)] : null;
        }

        /// <summary>
        /// Returns log level by provided marker or null if marker not available.
        /// </summary>
        public LogLevel? GetLogLevelByMarker(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            int specialCharactersCount = -1;
            int position = text.Length - 1;
            while (text[position--] == LogLevelMarkerSpecialCharacter)
            {
                ++specialCharactersCount;
            }
            return specialCharactersCount < 0 ? (LogLevel?)null : (LogLevel?)specialCharactersCount;
        }
    }
}
