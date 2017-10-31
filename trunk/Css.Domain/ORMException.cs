using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// ORM影射相关问题抛出此异常
    /// </summary>
    [Serializable]
    public class ORMException : AppException
    {
        /// <summary>
        /// 
        /// </summary>
        public ORMException() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ORMException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ORMException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ORMException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
