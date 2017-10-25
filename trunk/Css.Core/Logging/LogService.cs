using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Logging
{
    public static class LogService
    {
        static ILog log = GetLogger("app_logger");

        /// <summary>
        /// 默认Logger,名称为app_logger
        /// </summary>
        public static ILog Logger { get { return log; } }

        static ILoggerFactoryAdapter factory;

        static ILoggerFactoryAdapter Factory
        {
            get { return factory ?? (factory = new StreamWriterLoggerFactory()); }
        }

        /// <summary>
        /// 设置ILoggerFactoryAdapter
        /// </summary>
        /// <param name="loggerFactoryAdapter"></param>
        public static void SetFactory(ILoggerFactoryAdapter loggerFactoryAdapter)
        {
            factory = loggerFactoryAdapter;
            log = GetLogger("app_logger");
        }

        /// <summary>
        /// Gets the logger by calling <see cref="ILoggerFactoryAdapter.GetLogger(Type)"/>
        /// on the currently configured using the specified type.
        /// </summary>
        /// <returns>the logger instance obtained from the current</returns>
        public static ILog GetLogger<T>()
        {
            return Factory.GetLogger(typeof(T));
        }

        /// <summary>
        /// Gets the logger by calling <see cref="ILoggerFactoryAdapter.GetLogger(Type)"/>
        /// on the currently configured using the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>the logger instance obtained from the current</returns>
        public static ILog GetLogger(Type type)
        {
            return Factory.GetLogger(type);
        }

        /// <summary>
        /// Gets the logger by calling <see cref="ILoggerFactoryAdapter.GetLogger(string)"/>
        /// on the currently configured using the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>the logger instance obtained from the current</returns>
        public static ILog GetLogger(string key)
        {
            return Factory.GetLogger(key);
        }
    }
}
