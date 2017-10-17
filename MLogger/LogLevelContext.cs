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
        LogLevel LogLevel { get; set; }

        /// <summary>
        /// Log level block position in log file (end of the block).
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Log level marker (used to mark messages belongingness in log file).
        /// </summary>
        string Marker { get; set; }

        /// <summary>
        /// Contains messages to process.
        /// </summary>
        //ConcurrentBag<string> MessagesQueue { get; set; }

        /// <summary>
        /// Pause task token
        /// </summary>
        PauseTokenSource PauseToken { get; set; }

    }
}
