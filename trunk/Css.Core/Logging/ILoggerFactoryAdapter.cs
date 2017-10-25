using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Logging
{
    /// <summary>
    /// LoggerFactoryAdapter interface is used internally by LogManager
    /// Only developers wishing to write new Common.Logging adapters need to
    /// worry about this interface.
    /// </summary>
    /// <author>Gilles Bayon</author>
    public interface ILoggerFactoryAdapter
    {

        /// <summary>
        /// Get a ILog instance by type.
        /// </summary>
        /// <param name="type">The type to use for the logger</param>
        /// <returns></returns>
		ILog GetLogger(Type type);

        /// <summary>
        /// Get a ILog instance by key.
        /// </summary>
        /// <param name="key">The key of the logger</param>
        /// <returns></returns>
		ILog GetLogger(string key);
    }
}
