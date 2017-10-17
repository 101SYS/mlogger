using Nito.AsyncEx;
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
                this.LogLevelContexts = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>()
                    .OrderBy(ll => (byte)ll)
                    .Select(ll =>
                        new LogLevelContext()
                        {
                            LogLevel = ll,
                            Position = 0L,
                            Marker = new string(Enumerable.Repeat(LogLevelMarkerSpecialCharacter, ((int)ll) + 1).ToArray()),
                            PauseToken = new PauseTokenSource()
                        }).ToList().AsReadOnly();

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
