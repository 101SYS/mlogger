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
            if (String.IsNullOrWhiteSpace(logFilePath))
            {
                throw new ArgumentException("Log file path cannot be empty.", "logFilePath");
            }

            //Init
            this.LogFilePath = logFilePath;
            this.MessageProcessedAction = messageProcessedAction;
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));


            //Init log file
            string line;
            using (var sr = NewStreamReader)
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //TODO: update positions
                }
            }

        }
    }
}
