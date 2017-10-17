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
        #region IO
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

        protected FileInfo LogFileInfo
        {
            get { return new FileInfo(this.LogFilePath); }
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
            UpdatePositions(logLevel, textLength);
        }

        /// <summary>
        /// Updates changed log-level blocks positions, accordingly to saved text log-level
        /// </summary>
        /// <param name="logLevel">saved text log-level</param>
        /// <param name="text">saved text</param>
        protected void UpdatePositions(LogLevel? logLevel, int textLength)
        {
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
                    await llContext.PauseToken.Token.WaitWhilePausedAsync(); //waiting for higher levels
                    PauseLowerLogLevels(true, logLevel); //pause lower levels
                    lock (this) //one save at the time
                    {
                        InsertLogLevelMessage(llContext, message);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    await llContext.PauseToken.Token.WaitWhilePausedAsync(); //waiting for higher level
                    PauseLowerLogLevels(false, logLevel); //unpause lower levels
                }
            }
            else
            {
                lock (this) //one direct, time-consistent save at the time
                {
                    using (var sw = NewAppender)
                    {
                        sw.WriteLine(message);
                    }
                }
            }
            MessageProcessedAction?.Invoke(message, logLevel);
        }

        /// <summary>
        /// Saving message at the end of its log-level block. 
        /// Steps: 
        /// 1. Increasing file size by message bytes count. 
        /// 2. Moving all the data from last message in appropriate log-level messages block (opening gap). 
        /// 3. Inserting new message. 
        /// </summary>
        /// <param name="llContext">Message LogLevelContext</param>
        /// <param name="message">Message, including markers</param>
        /// <returns></returns>
        private void InsertLogLevelMessage(LogLevelContext llContext, string message)
        {
            message += llContext.Marker + Environment.NewLine;
            byte[] messageBytes = Configuration.Current.LogFileEncoding.GetBytes(message);
            long messageLength = messageBytes.LongLength;
            long fileInitialLength = LogFileInfo.Length;
            long slidingBlockLenght = fileInitialLength - llContext.Position;
            int bufferSize = (int)Math.Min(10485760L/*10MB*/, slidingBlockLenght);
            int readsCount = (int)(slidingBlockLenght / bufferSize);
            int remainingBytes = (int)(slidingBlockLenght % bufferSize);
            byte[] bufferRead = new byte[bufferSize];
            byte[] bufferWrite = new byte[bufferSize];

            using (var sw = NewWriter)
            {
                sw.BaseStream.SetLength(LogFileInfo.Length + messageBytes.LongLength);
                using (var sr = NewReader)
                {
                    sr.BaseStream.Seek(llContext.Position, SeekOrigin.Begin);
                    sw.BaseStream.Seek(llContext.Position + messageLength, SeekOrigin.Begin);
                    sr.BaseStream.Read(bufferRead, 0, bufferSize);
                    for (int i = 1; i < readsCount; i++)
                    {
                        bufferRead.CopyTo(bufferWrite, 0);
                        sr.BaseStream.Read(bufferRead, 0, bufferSize);
                        sw.BaseStream.Write(bufferWrite, 0, bufferSize);
                    }
                    bufferRead.CopyTo(bufferWrite, 0);
                    if (remainingBytes > 0)
                    {
                        bufferRead = new byte[remainingBytes];
                        sr.BaseStream.Read(bufferRead, 0, remainingBytes);
                        sw.BaseStream.Write(bufferWrite, 0, bufferSize);
                        bufferWrite = new byte[remainingBytes];
                        bufferRead.CopyTo(bufferWrite, 0);
                        bufferSize = remainingBytes;
                    }
                }
                sw.BaseStream.Write(bufferWrite, 0, bufferSize);
                bufferRead = bufferWrite = null;

                sw.BaseStream.Seek(llContext.Position, SeekOrigin.Begin);
                sw.Write(messageBytes);
            }
            UpdatePositions(llContext.LogLevel, messageBytes.Length);
        }
    }
}
