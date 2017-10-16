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
        protected StreamReader NewStreamReader
        {
            get
            {
                return new StreamReader(
                File.Open(Configuration.Current.LogFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite),
                Configuration.Current.LogFileEncoding, true);
            }
        }

        protected StreamWriter NewStreamWriter
        {
            get
            {
                return new StreamWriter(
                File.Open(Configuration.Current.LogFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read),
                Configuration.Current.LogFileEncoding);
            }
        }

        
    }
}
