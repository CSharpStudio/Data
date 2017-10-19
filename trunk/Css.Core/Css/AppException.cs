using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Css
{
    /// <summary>
    /// 应用异常，最基本的应用程序定义的异常
    /// </summary>
    [Serializable]
    public class AppException : ApplicationException
    {
        /// <summary>
        /// 应用异常构造器
        /// </summary>
        public AppException() : base() { }
        /// <summary>
        /// 应用异常构造器
        /// </summary>
        /// <param name="message">异常信息</param>
        public AppException(string message) : base(message) { }
        /// <summary>
        /// 应用异常构造器
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected AppException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
        /// <summary>
        /// 应用异常构造器
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">内部异常</param>
        public AppException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

