using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MLogger.Web
{
    [ServiceContract]
    public interface ILoggerSvc
    {

        [OperationContract]
        void Log(string message, LogLevel logLevel);
    }
}
