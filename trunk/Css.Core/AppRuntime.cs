using Css.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css
{
    public class AppRuntime
    {
        static AppRuntime()
        {
            Config = new ConfigurationBuilder().AddJsonFile("config.json").Build();
        }

        public static IConfiguration Config { get; }

        /// <summary>
        /// 服务容器
        /// </summary>
        public static IServiceContainer Service { get; } = new ServiceContainer();
    }

    public class RT : AppRuntime { }
}
