using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 引用实体属性的实体标记
    /// </summary>
    /// <typeparam name="TRefEntity">引用实体的类型</typeparam>
    public sealed class RefEntityProperty<TRefEntity> : Property<TRefEntity>, IRefEntityProperty
        where TRefEntity : Entity
    {
        IRefIdProperty _refIdProperty;

        /// <summary>
        /// 自定义加载器。
        /// </summary>
        RefEntityLoader _loader;

        /// <summary>
        /// 为了提高性能，在这个属性上添加一个 IRepository 的缓存字段。
        /// </summary>
        IRepository _defaultLoader;

        public RefEntityProperty(Type ownerType, Type declareType, string propertyName, bool serializable) : base(ownerType, declareType, propertyName, serializable) { }

        public RefEntityProperty(Type ownerType, string propertyName, bool serializable) : base(ownerType, propertyName, serializable) { }

        public override PropertyCategory Category
        {
            get { return PropertyCategory.ReferenceEntity; }
        }

        public ReferenceType ReferenceType
        {
            get { return _refIdProperty.ReferenceType; }
        }

        public IRefIdProperty RefIdProperty
        {
            get { return _refIdProperty; }
            internal set
            {
                _refIdProperty = value;
                if (value != null)
                    value.RefEntityProperty = this;
            }
        }

        /// <summary>
        /// 自定义的引用实体加载器。
        /// </summary>
        public RefEntityLoader Loader
        {
            get { return _loader; }
            internal set { _loader = value; }
        }

        public bool Nullable
        {
            get { return _refIdProperty.Nullable; }
        }

        IRefEntityProperty IRefProperty.RefEntityProperty
        {
            get { return this; }
        }

        IEntity IRefEntityProperty.Load(object id, IEntity owner)
        {
            //通过自定义 Loader 获取实体。
            if (_loader != null)
                return _loader(id, owner);

            //通过默认的 CacheById 方法获取实体。
            if (_defaultLoader == null)
                _defaultLoader = RF.Find(PropertyType);
            return _defaultLoader.GetById(id);
        }
    }

    /// <summary>
    /// 引用实体加载器方法。
    /// </summary>
    /// <param name="id">引用实体的 id。</param>
    /// <param name="owner">拥有该引用属性的实体。</param>
    /// <returns>返回对应的引用实体。</returns>
    public delegate IEntity RefEntityLoader(object id, IEntity owner);
}
