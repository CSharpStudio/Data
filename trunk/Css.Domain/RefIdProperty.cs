using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 引用实体属性的 Id 标记
    /// </summary>
    public sealed class RefIdProperty<TKey> : Property<TKey>, IRefIdProperty
    {
        IRefEntityProperty _refEntityProperty;

        IKeyProvider _keyProvider;

        public RefIdProperty(Type ownerType, Type declareType, string propertyName, bool serializable) : base(ownerType, declareType, propertyName, serializable) { }

        public RefIdProperty(Type ownerType, string propertyName, bool serializable) : base(ownerType, propertyName, serializable) { }

        public override PropertyCategory Category
        {
            get { return PropertyCategory.ReferenceId; }
        }

        /// <summary>
        /// 实体引用的类型
        /// </summary>
        public ReferenceType ReferenceType { get; internal set; }

        /// <summary>
        /// 该引用属性是否可空
        /// </summary>
        public bool Nullable { get; internal set; }

        /// <summary>
        /// 返回对应的引用实体属性。
        /// </summary>
        public IRefEntityProperty RefEntityProperty
        {
            get
            {
                CheckRefEntityProperty();
                return _refEntityProperty;
            }
            set { _refEntityProperty = value; }
        }

        void CheckRefEntityProperty()
        {
            if (_refEntityProperty == null)
                throw new ORMException("没有为[{0}.{1}]属性编写对应的实体引用属性".FormatArgs(OwnerType.Name, Name));
        }

        /// <summary>
        /// 引用的实体的主键的算法程序。
        /// </summary>
        public IKeyProvider KeyProvider
        {
            get { return _keyProvider ?? (_keyProvider = KeyProviders.Get(PropertyType)); }
        }

        IRefIdProperty IRefProperty.RefIdProperty
        {
            get { return this; }
        }
    }
}
