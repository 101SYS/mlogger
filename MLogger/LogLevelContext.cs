using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLogger
{
    /// <summary>
    /// Message processing context
    /// </summary>
    internal class LogLevelContext
    {
        /// <summary>
        /// Corresponding log level.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Log level block position in log file (end of the block).
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// Log level marker (used to mark messages belongingness in log file).
        /// </summary>
        public string Marker { get; set; }

        /// <summary>
        /// Pause task token
        /// </summary>
        public PauseTokenSource PauseToken { get; set; }

    }
}
