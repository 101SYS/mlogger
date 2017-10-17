using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLogger
{
    /// <summary>
    /// Log entry level (severity) enumerator.
    /// </summary>
    public enum LogLevel : byte
    {
        Critical = 0,
        Error,
        Warn,
        Info,
        Debug
    }
}
