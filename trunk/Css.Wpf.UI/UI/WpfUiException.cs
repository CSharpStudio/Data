using System;
using System.Runtime.Serialization;

namespace Css.Wpf.UI
{
    [Serializable]
    public class WpfUiException : Exception
    {
        public WpfUiException()
        {
        }

        public WpfUiException(string message)
            : base(message)
        {
        }

        public WpfUiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected WpfUiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}