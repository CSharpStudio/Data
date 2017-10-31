using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    /// <summary>
    /// 构建实体查询<see cref="IQuery"/>失败时抛出此异常
    /// </summary>
    [Serializable]
    public class QueryException : AppException
    {
        /// <summary>
        /// 
        /// </summary>
        public QueryException() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public QueryException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected QueryException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public QueryException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
