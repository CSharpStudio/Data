using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Css.Configuration
{
    public static class ConfigurationManager
    {
        static IConfiguration _config;

        public static NameValueCollection AppSettings { get; }

        public static NameValueCollection ConnectionStrings { get; }
    }
}
