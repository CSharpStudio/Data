using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Common
{
    /// <summary>
    /// 参数太多时抛出此异常，例如Oracle In语言中参数超过1000个会抛此异常
    /// </summary>
    [Serializable]
    public class TooManyParameterException : SqlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyParameterException"/> class.
        /// </summary>
        public TooManyParameterException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyParameterException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected TooManyParameterException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
