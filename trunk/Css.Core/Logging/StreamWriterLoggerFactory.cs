using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Logging
{
    class StreamWriterLoggerFactory : ILoggerFactoryAdapter
    {
        StreamWriterLogger logger = new StreamWriterLogger();

        public ILog GetLogger(string key)
        {
            return logger;
        }

        public ILog GetLogger(Type type)
        {
            return logger;
        }
    }
}
