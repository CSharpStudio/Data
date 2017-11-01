using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public interface IProperty : IVarProperty
    {
        PropertyCategory Category { get; }
    }

    public interface IListProperty : IProperty
    {
        HasManyType HasManyType { get; }
        /// <summary>
        /// 自定义列表数据提供器
        /// </summary>
        ListLoaderProvider DataProvider { get; }

        Type EntityType { get; }
    }

    public interface IListProperty<T> : IListProperty
    {
    }

    public interface IRefProperty : IProperty
    {
        /// <summary>
        /// 实体引用的类型
        /// </summary>
        ReferenceType ReferenceType { get; }

        /// <summary>
        /// 该引用属性是否可空。
        /// 如果引用Id属性的类型是引用类型（字符串）或者是一个 Nullable 类型，则这个属性返回 true。
        /// </summary>
        bool Nullable { get; }

        /// <summary>
        /// 返回对应的引用 Id 属性。
        /// </summary>
        IRefIdProperty RefIdProperty { get; }

        /// <summary>
        /// 返回对应的引用实体属性。
        /// </summary>
        IRefEntityProperty RefEntityProperty { get; }
    }

    public interface IRefIdProperty : IRefProperty
    {
        /// <summary>
        /// 引用的实体的主键的算法程序。
        /// </summary>
        IKeyProvider KeyProvider { get; }
        /// <summary>
        /// 返回对应的引用实体属性。
        /// </summary>
        new IRefEntityProperty RefEntityProperty { get; set; }
    }

    /// <summary>
    /// 引用实体属性的静态属性实体标记
    /// </summary>
    public interface IRefEntityProperty : IRefProperty
    {
        IEntity Load(object id, IEntity owner);
    }
}
