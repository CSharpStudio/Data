using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public sealed class ListProperty<TEntityList> : Property<TEntityList>, IListProperty<TEntityList>
        where TEntityList : IEntityList
    {
        public ListProperty(Type ownerType, Type declareType, string propertyName, bool serializable)
            : base(ownerType, declareType, propertyName, serializable) { }

        public ListProperty(Type ownerType, string propertyName, bool serializable)
            : base(ownerType, propertyName, serializable) { }

        public override PropertyCategory Category
        {
            get { return PropertyCategory.List; }
        }

        /// <summary>
        /// 一对多子属性的类型
        /// </summary>
        public HasManyType HasManyType { get; set; }
        /// <summary>
        /// 自定义列表数据提供器
        /// </summary>
        public ListLoaderProvider DataProvider { get; set; }

        Type _entityType;
        public Type EntityType
        {
            get { return _entityType ?? (_entityType = PropertyType.GetGenericType(typeof(EntityList<>)).GetGenericArguments()[0]); }
        }
    }

    /// <summary>
    /// 列表数据提供程序
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <returns></returns>
    public delegate IEntityList ListLoaderProvider(Entity owner);
}