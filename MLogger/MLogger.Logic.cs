using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MLogger
{
    public partial class MLogger
    {
        #region Streams
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
        #endregion

        /// <summary>
        /// Returns LogLevelContext by provided LogLevel
        /// </summary>
        /// <param name="logLevel">log level</param>
        /// <returns>LogLevelContext by provided LogLevel</returns>
        private LogLevelContext GetLogLevelContext(LogLevel logLevel)
        {
            return this.LogLevelContexts[(int)logLevel];
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
            for (int i = LogLevelContexts.Count - 1; i >= logLevelIndex; i--)
            {
                LogLevelContexts[i].Position += textLength;
            }
        }

        /// <summary>
        /// Pause or unpause lower log levels (lower severities)
        /// </summary>
        /// <param name="pause">true - pause, false - unpause</param>
        private void PauseLowerLogLevels(bool pause, LogLevel logLevel)
        {
            int logLevelIndex = (int)logLevel;
            for (int i = LogLevelContexts.Count - 1; i > logLevelIndex; i--)
            {
                LogLevelContexts[i].PauseToken.IsPaused = pause;
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


        /// <summary>
        /// Returns log level marker by provided log level. 
        /// If OrderEntriesByLogLevel is true - should be added to message to specify its log level.
        /// </summary>
        public string GetLogLevelMarker(LogLevel logLevel)
        {
            return Configuration.Current.OrderEntriesByLogLevel ? GetLogLevelContext(logLevel).Marker : null;
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



        /// <summary>
        /// Process message asynchronously. 
        /// Save message sequentially (append message) or passes it for forwarder asynchronous processing.
        /// </summary>
        /// <param name="logLevel">message log level</param>
        /// <param name="message">message</param>
        protected async void ProcessMessageAsync(LogLevel logLevel, string message)
        {
            if (Configuration.Current.OrderEntriesByLogLevel)
            {
                var llContext = GetLogLevelContext(logLevel);
                try
                {
                    await llContext.PauseToken.Token.WaitWhilePausedAsync(); //waiting for higher level
                    PauseLowerLogLevels(true, logLevel); //pause lower level
                    lock (this) //one save at the time
                    {
                        SaveLogLevelMessage(llContext, message + llContext.Marker);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    PauseLowerLogLevels(false, logLevel); //unpause lower level
                }
            }
            else //direct, time-consistent save
            {
                using (var sw = NewAppender)
                {
                    await sw.WriteLineAsync(message);
                }
            }
            MessageProcessedAction?.Invoke(message, logLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="llContext"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private void SaveLogLevelMessage(LogLevelContext llContext, string message)
        {
            //TODO: Implement
        }
    }
}
