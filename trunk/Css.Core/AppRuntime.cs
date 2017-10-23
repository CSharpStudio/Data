using Css.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css
{
    public class AppRuntime
    {
        /// <summary>
        /// 服务容器
        /// </summary>
        public static IServiceContainer Service { get; } = new ServiceContainer();
    }

    public class RT : AppRuntime { }
}
