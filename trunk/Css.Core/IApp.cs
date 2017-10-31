using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css
{
    public interface IApp
    {
        /// <summary>
        /// 模块初始化。
        /// </summary>
        event EventHandler ModuleIntialized;

        /// <summary>
        /// 服务初始化。
        /// </summary>
        event EventHandler ServiceIntializing;

        /// <summary>
        /// 应用程序运行时行为开始。
        /// </summary>
        event EventHandler RuntimeStarting;

        /// <summary>
        /// 主过程开始前事件。
        /// </summary>
        event EventHandler MainProcessStarting;

        /// <summary>
        /// AppStartup 完毕
        /// </summary>
        event EventHandler StartupCompleted;

        /// <summary>
        /// 应用程序完全退出
        /// </summary>
        event EventHandler Exit;
    }
}
