using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Modules
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ModuleAttribute : Attribute
    {
        public Type ModuleType { get; }

        public ModuleAttribute(Type moduleType)
        {
            Check.NotNull(moduleType, nameof(moduleType));

            if (!typeof(IModule).IsAssignableFrom(moduleType))
                throw new ArgumentException("参数{0}类型{1}未实现IModule接口".FormatArgs(nameof(moduleType), moduleType.Name));

            ModuleType = moduleType;
        }

        public ModuleAttribute() { }
    }
}
