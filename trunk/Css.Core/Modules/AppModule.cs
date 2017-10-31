using System.Reflection;

namespace Css.Modules
{
    public abstract class AppModule : IModule
    {
        /// <summary>
        /// 插件对应的程序集。
        /// </summary>
        public Assembly Assembly
        {
            get { return GetType().Assembly; }
        }

        /// <summary>
        /// 插件的初始化方法。
        /// 框架会在启动时根据启动级别顺序调用本方法。
        /// 
        /// 方法有两个职责：
        /// 1.依赖注入。
        /// 2.注册 app 生命周期中事件，进行特定的初始化工作。
        /// </summary>
        /// <param name="app">应用程序对象。</param>
        public abstract void Initialize(IApp app);
    }
}
