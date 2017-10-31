using Css.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Runtime
{
    public class AppBase : IApp
    {
        public event EventHandler Exit;
        public event EventHandler MainProcessStarting;
        public event EventHandler ModuleIntialized;
        public event EventHandler RuntimeStarting;
        public event EventHandler ServiceIntializing;
        public event EventHandler StartupCompleted;

        protected virtual void OnExit()
        {
            Exit?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMainProcessStarting()
        {
            MainProcessStarting?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnModuleIntialized()
        {
            ModuleIntialized?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnRuntimeStarting()
        {
            RuntimeStarting?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnServiceIntializing()
        {
            ServiceIntializing?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStartupCompleted()
        {
            StartupCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the logger assosiated with this object.
        /// </summary>
        public ILog Logger { get; set; }

        protected void StartupApplication()
        {
            Logger = LogService.GetLogger("platform_logger");
            Logger.Info("程序开始启动");
            AppRuntime.App = this;
            OnIntializing();

            AppRuntime.StartupModules();
            OnModuleIntialized();
            Logger.Info("模块初始化完成");

            OnServiceIntializing();
            Logger.Info("服务初始化完成");

            OnRuntimeStarting();
            Logger.Info("运行时已启动");

            OnMainProcessStarting();
            StartMainProcess();
            Logger.Info("主程序已启动");

            OnStartupCompleted();
            Logger.Info("程序启动完成");
        }

        /// <summary>
        /// 子类重写此方法实现启动主逻辑。
        /// </summary>
        protected virtual void StartMainProcess() { }

        protected virtual void OnIntializing() { }
    }
}
