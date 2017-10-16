using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLogger.Engine
{
    /// <summary>
    /// Log entry level (severity) flags.
    /// </summary>
    public enum LogLevel : byte
    {
        Critical,
        Error,
        Warn,
        Info,
        Debug
    }
}
