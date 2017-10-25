using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Logging
{
    public class StreamWriterLogger : ILog
    {
        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public bool IsFatalEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsTraceEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public void Debug(object message)
        {
            Write("Debug\r\n" + message);
        }

        public void Debug(object message, Exception exception)
        {
            Write("Debug\r\n" + message + "\r\n" + exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Write("Debug\r\n" + format.FormatArgs(args));
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Write("Debug\r\n" + format.FormatArgs(args));
        }

        public void DebugFormat(string format, Exception exception, params object[] args)
        {
            Write("Debug\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Write("Debug\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void Error(object message)
        {
            Write("Error\r\n" + message);
        }

        public void Error(object message, Exception exception)
        {
            Write("Error\r\n" + message + "\r\n" + exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Write("Error\r\n" + format.FormatArgs(args));
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Write("Error\r\n" + format.FormatArgs(args));
        }

        public void ErrorFormat(string format, Exception exception, params object[] args)
        {
            Write("Error\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Write("Error\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void Fatal(object message)
        {
            Write("Fatal\r\n" + message);
        }

        public void Fatal(object message, Exception exception)
        {
            Write("Fatal\r\n" + message + "\r\n" + exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            Write("Fatal\r\n" + format.FormatArgs(args));
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Write("Fatal\r\n" + format.FormatArgs(args));
        }

        public void FatalFormat(string format, Exception exception, params object[] args)
        {
            Write("Fatal\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Write("Fatal\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void Info(object message)
        {
            Write("Info\r\n" + message);
        }

        public void Info(object message, Exception exception)
        {
            Write("Info\r\n" + message + "\r\n" + exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Write("Info\r\n" + format.FormatArgs(args));
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Write("Info\r\n" + format.FormatArgs(args));
        }

        public void InfoFormat(string format, Exception exception, params object[] args)
        {
            Write("Info\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Write("Info\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void Trace(object message)
        {
            Write("Trace\r\n" + message);
        }

        public void Trace(object message, Exception exception)
        {
            Write("Trace\r\n" + message + "\r\n" + exception);
        }

        public void TraceFormat(string format, params object[] args)
        {
            Write("Trace\r\n" + format.FormatArgs(args));
        }

        public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Write("Trace\r\n" + format.FormatArgs(args));
        }

        public void TraceFormat(string format, Exception exception, params object[] args)
        {
            Write("Trace\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void TraceFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Write("Trace\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void Warn(object message)
        {
            Write("Warn\r\n" + message);
        }

        public void Warn(object message, Exception exception)
        {
            Write("Warn\r\n" + message + "\r\n" + exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Write("Warn\r\n" + format.FormatArgs(args));
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Write("Warn\r\n" + format.FormatArgs(args));
        }

        public void WarnFormat(string format, Exception exception, params object[] args)
        {
            Write("Warn\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Write("Warn\r\n" + format.FormatArgs(args) + "\r\n" + exception);
        }

        void Write(string msg)
        {
            using (var sw = new System.IO.StreamWriter("app.log", true))
            {
                sw.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff:"));
                sw.WriteLine(msg);
            }
        }
    }
}

