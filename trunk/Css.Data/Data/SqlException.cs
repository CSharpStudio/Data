﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Css.Data
{
    /// <summary>
    /// SQL执行失败的异常
    /// </summary>
    [Serializable]
    public class SqlException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlException"/> class.
        /// </summary>
        public SqlException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public SqlException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlException"/> class.
        /// </summary>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the innerException parameter 
        /// is not a null reference, the current exception is raised in a catch block that handles 
        /// the inner exception.
        /// </param>
        public SqlException(Exception innerException) : base(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the innerException parameter 
        /// is not a null reference, the current exception is raised in a catch block that handles 
        /// the inner exception.
        /// </param>
        public SqlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlException"/> class 
        /// with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object 
        /// data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected SqlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}