using Css.Configuration;
using Css.IO;
using Css.Logging;
using Css.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Css
{
    public class AppRuntime
    {
        static AppRuntime()
        {
            Config = new ConfigBuilder().LoadXmlFile("appSettings.config").Build();
        }

        /// <summary>
        /// 配置文件保存在运行目录
        /// </summary>
        public static IConfig Config { get; }

        /// <summary>
        /// 服务容器
        /// </summary>
        public static IServiceContainer Service { get; } = new ServiceContainer();

        /// <summary>
        /// 日志
        /// </summary>
        public static ILog Logger { get { return LogService.Logger; } }
    }

    public class RT : AppRuntime { }
}
