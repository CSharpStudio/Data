using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.Modules
{
    public class ModuleAssembly
    {
        public ModuleAssembly(Assembly assembly, IModule module)
        {
            Module = module;
            Assembly = assembly;
        }

        /// <summary>
        /// 程序集当中的插件对象。
        /// 如果插件中没有定义，则此属性为 null。
        /// </summary>
        public IModule Module { get; set; }

        /// <summary>
        /// 程序集本身
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// 本属性表示插件在所有插件中的启动索引号。
        /// 索引号表示了插件的启动优先级，索引号越小，越先被启动。
        /// 该优先级的计算方式为：
        /// 
        /// 1. 所有 DomainPlugin 的索引号全部少于所有的 UIPlugin 的索引号；
        /// 2. 接着按照 SetupLevel 进行排序，越小的 SetupLevel 对应越小的索引号。
        /// 3. 对于 SetupLevel 相同的插件，则根据引用关系对插件进行排序，引用其它插件越少的插件，对应的索引号更小。
        /// </summary>
        public int SetupIndex { get; internal set; }
    }

    internal class EmptyModule : IModule
    {
        Assembly _assembly;

        public EmptyModule(Assembly assembly)
        {
            _assembly = assembly;
        }

        /// <summary>
        /// 插件对应的程序集。
        /// </summary>
        public Assembly Assembly
        {
            get { return _assembly; }
        }

        public void Initialize(IApp app) { }
    }
}
