using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLogger
{
    public partial class MLogger
    {
        /// <summary>
        /// Gets or Sets action, executed after message is processed.
        /// </summary>
        public Action<string, LogLevel> MessageProcessedAction { get; set; }

        /// <summary>
        /// Log file path, specified at instance construction.
        /// </summary>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// list of log level contexts. 
        /// Item index correlates to LogLevel.
        /// </summary>
        private IReadOnlyList<LogLevelContext> LogLevelContexts { get; set; }

        /// <summary>
        /// New line characters reversed (to trim)
        /// </summary>
        private static readonly char[] NewLineCharacters = Environment.NewLine.ToCharArray().Reverse().ToArray();

        /// <summary>
        /// Log level marker character
        /// </summary>
        public const char LogLevelMarkerSpecialCharacter = (char)0;

        /// <summary>
        /// Log message. If LogLevel not specified - logging message as info.
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="logLevel">message log level (severity)</param>
        /// <returns>MLogger</returns>
        public MLogger Log(string message, LogLevel logLevel = LogLevel.Info)
        {
            return Log(logLevel, message);
        }

        /// <summary>
        /// Format and log message with optional additional information.
        /// </summary>
        /// <param name="logLevel">message log level (severity)</param>
        /// <param name="message">message to log</param>
        /// <param name="additionalInfo">optional additional information</param>
        /// <returns>MLogger</returns>
        protected MLogger Log(LogLevel logLevel, object message, params object[] additionalInfo)
        {
            if (IsLogLevelEnabled(logLevel))
            {
                string info = (additionalInfo != null && additionalInfo.Length > 0) ? string.Join<object>(", ", additionalInfo) : string.Empty;
                string msg = Configuration.Current.MessageFormat
                    .Replace("$(TimeStamp)", DateTime.Now.ToString(Configuration.Current.MessageTimeStampFormat))
                    .Replace("$(NewLine)", Environment.NewLine)
                    .Replace("$(LogLevel)", logLevel.ToString())
                    .Replace("$(Message)", message.ToString())
                    .Replace("$(AdditionalInfo)", info)
                    .Trim();

                ProcessMessageAsync(logLevel, msg);
            }

            return this;
        }

        /// <summary>
        /// Format and log debug-level message with optional additional information.
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="additionalInfo">optional additional information</param>
        /// <returns>MLogger</returns>
        public MLogger LogDebug(object message, params object[] additionalInfo)
        {
            return Log(LogLevel.Debug, message, additionalInfo);
        }

        /// <summary>
        /// Format and log info-level message with optional additional information.
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="additionalInfo">optional additional information</param>
        /// <returns>MLogger</returns>
        public MLogger LogInfo(object message, params object[] additionalInfo)
        {
            return Log(LogLevel.Info, message, additionalInfo);
        }

        /// <summary>
        /// Format and log warn-level message with optional additional information.
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="additionalInfo">optional additional information</param>
        /// <returns>MLogger</returns>
        public MLogger LogWarn(object message, params object[] additionalInfo)
        {
            return Log(LogLevel.Warn, message, additionalInfo);
        }

        /// <summary>
        /// Format and log error-level message with optional additional information.
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="additionalInfo">optional additional information</param>
        /// <returns>MLogger</returns>
        public MLogger LogError(object message, params object[] additionalInfo)
        {
            return Log(LogLevel.Error, message, additionalInfo);
        }

        /// <summary>
        /// Format and log critical-level message with optional additional information.
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="additionalInfo">optional additional information</param>
        /// <returns>MLogger</returns>
        public MLogger LogCritical(object message, params object[] additionalInfo)
        {
            return Log(LogLevel.Critical, message, additionalInfo);
        }
    }
}
