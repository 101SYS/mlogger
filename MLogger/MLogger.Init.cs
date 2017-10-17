using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLogger
{
    public partial class MLogger
    {
        static MLogger()
        {
            Default = new MLogger();
        }

        /// <summary>
        /// Default static logger.
        /// </summary>
        public readonly static MLogger Default;

        /// <summary>
        /// Create new logger instance with default file path from configuration
        /// </summary>
        public MLogger() : this(Configuration.Current.LogFilePath)
        { }


        /// <summary>
        /// Create new logger instance with custom file path and optional message-processed action
        /// </summary>
        public MLogger(string logFilePath, Action<string, LogLevel> messageProcessedAction = null)
        {
            //Verify parameters
            if (String.IsNullOrWhiteSpace(logFilePath))
            {
                throw new ArgumentException("Log file path cannot be empty.", "logFilePath");
            }

            //Initialize variables
            this.LogFilePath = logFilePath;
            this.MessageProcessedAction = messageProcessedAction;
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            if (Configuration.Current.OrderEntriesByLogLevel)
            {
                LogLevelMarkers = Enum.GetValues(typeof(LogLevel)).Cast<byte>().OrderBy(value => value)
                    .Select(value => new string(Enumerable.Repeat(LogLevelMarkerSpecialCharacter, value + 1).ToArray()))
                    .ToList().AsReadOnly();
                Positions = Enumerable.Repeat(0L, LogLevelMarkers.Count).ToList();
                byte logLevel = 0;
                MessagesQueues = new ConcurrentDictionary<LogLevel, ConcurrentBag<string>>(
                    Enumerable.Repeat(
                        new KeyValuePair<LogLevel, ConcurrentBag<string>>((LogLevel)logLevel++, new ConcurrentBag<string>()), Positions.Count).ToArray());


                //Initialize log file and set log-level blocks positions
                using (var sr = NewReader)
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine() ?? string.Empty;
                        UpdatePositions(line.TrimEnd(NewLineCharacters));
                    }
                }
            }

        }
    }
}
