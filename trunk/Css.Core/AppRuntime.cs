using Css.Configuration;
using Css.IO;
using Css.Logging;
using Css.Resources;
using Css.Runtime;
using Css.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Css
{
    public partial class AppRuntime
    {
        static AppRuntime()
        {
            Config = new ConfigBuilder().LoadXmlFile("appSettings.config").Build();
        }

        /// <summary>
        /// 当前的应用程序运行时。
        /// </summary>
        public static IApp App { get; internal set; }

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

        /// <summary>
        /// 资源服务
        /// </summary>
        public static IResourceService ResourceService { get { return Resources.ResourceService.Current; } }

        public static RuntimeEnvironment Environment { get => RuntimeEnvironment.Instance; }
    }

    public class RT : AppRuntime { }
}
