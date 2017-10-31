using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 标记某一个类型是指定的实体对应的仓库类型。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class RepositoryForAttribute : Attribute
    {
        public RepositoryForAttribute(Type entityType)
        {
            EntityType = entityType;
        }

        /// <summary>
        /// 对应的实体类型。
        /// </summary>
        public Type EntityType { get; private set; }
    }
}
