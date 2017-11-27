using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    /// <summary>
    /// 可变对象的属性
    /// </summary>
    public interface IVarProperty
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 所有者类型
        /// </summary>
        Type OwnerType { get; }
        /// <summary>
        /// 声明者类型
        /// </summary>
        Type DeclareType { get; }
        /// <summary>
        /// 属性类型
        /// </summary>
        Type PropertyType { get; }
        /// <summary>
        /// 是否只读
        /// </summary>
        bool IsReadOnly { get; }
        /// <summary>
        /// 获取只读值，如果当前属性是只读属性调用此方法可获取只读的值。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        object GetReadOnlyValue(VarObject obj);
        /// <summary>
        /// 类中编译索引。属性的值存储于字典中，通过此索引对值进行存取。
        /// </summary>
        int TypeCompiledIndex { get; }
        /// <summary>
        /// 全局索引，属性注册时的全局索引
        /// </summary>
        int GlobalIndex { get; }
        /// <summary>
        /// 只读属性的依赖。如果当前属性是只读属性，当依赖的属性值变更时，只读属性的值也触发变更通知。
        /// </summary>
        IList<VarPropertyBase> ReadonlyDependencies { get; }
        /// <summary>
        /// 是否需要序列化此属性
        /// </summary>
        bool Serializable { get; }
    }

    /// <summary>
    /// 可变对象的属性
    /// </summary>
    /// <typeparam name="T">属性类型参数</typeparam>
    public interface IVarProperty<T> : IVarProperty { }

    /// <summary>
    /// 可变对象的属性.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public abstract class VarPropertyBase : IVarProperty
    {
        /// <summary>
        /// Gets or sets the Name of the property.
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Gets or sets the owner type of the property.
        /// </summary>
        public Type OwnerType { get; protected set; }
        /// <summary>
        /// Gets or sets the declare type of the property.
        /// </summary>
        public Type DeclareType { get; protected set; }
        /// <summary>
        /// Gets or sets the type of the Property.
        /// </summary>
        public Type PropertyType { get; protected set; }
        /// <summary>
        /// Gets a value indicating whether the field is readonly.
        /// </summary>
        public abstract bool IsReadOnly { get; }
        
        /// <summary>
        /// 是否需要序列化此属性
        /// </summary>
        public bool Serializable { get; set; }

        /// <summary>
        /// 获取只读的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal abstract object GetReadOnlyValue(VarObject obj);

        object IVarProperty.GetReadOnlyValue(VarObject obj)
        {
            return GetReadOnlyValue(obj);
        }

        ///<inheritdoc cref="IVarProperty.TypeCompiledIndex"></inherikdoc>
        internal int TypeCompiledIndex { get; set; }

        /// <summary>
        /// 全局索引，属性注册时的全局索引
        /// </summary>
        internal int GlobalIndex { get; set; }

        /// <summary>
        /// 只读属性的依赖。如果当前属性是只读属性，当依赖的属性值变更时，只读属性的值也触发变更通知。
        /// </summary>
        internal IList<VarPropertyBase> ReadonlyDependencies { get; } = new List<VarPropertyBase>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        int IVarProperty.TypeCompiledIndex
        {
            get { return TypeCompiledIndex; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        int IVarProperty.GlobalIndex
        {
            get { return GlobalIndex; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        IList<VarPropertyBase> IVarProperty.ReadonlyDependencies
        {
            get { return ReadonlyDependencies; }
        }
    }

    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class VarProperty : VarPropertyBase
    {
        Func<VarObject, object> _readOnlyValueProvider;
        /// <summary>
        /// Gets a value indicating whether the field is readonly.
        /// </summary>
        public override bool IsReadOnly
        {
            get { return _readOnlyValueProvider != null; }
        }

        protected internal override object GetReadOnlyValue(VarObject obj)
        {
            return _readOnlyValueProvider(obj);
        }

        public VarProperty(Type ownerType, string propertyName, Type propertyType, bool serializable)
            : this(ownerType, ownerType, propertyName, propertyType, serializable) { }

        public VarProperty(Type ownerType, Type declareType, string propertyName, Type propertyType, bool serializable)
        {
            Check.NotNull(ownerType, nameof(ownerType));
            Check.NotNull(declareType, nameof(declareType));
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            Check.NotNull(propertyType, nameof(propertyType));

            TypeCompiledIndex = -1;
            GlobalIndex = -1;
            OwnerType = ownerType;
            DeclareType = declareType;
            Serializable = serializable;
            PropertyType = propertyType;
            Name = propertyName;
        }

        /// <summary>
        /// 转为只读属性
        /// </summary>
        /// <param name="readOnlyValueProvider"></param>
        /// <param name="dependencies"></param>
        public void AsReadOnly(Func<VarObject, object> readOnlyValueProvider, params VarPropertyBase[] dependencies)
        {
            if (GlobalIndex >= 0) throw new InvalidOperationException("属性已经注册完毕，不能修改！");

            Check.NotNull(readOnlyValueProvider, nameof(readOnlyValueProvider));

            _readOnlyValueProvider = readOnlyValueProvider;

            foreach (var property in dependencies)
            {
                if (!property.ReadonlyDependencies.Contains(this))
                    property.ReadonlyDependencies.Add(this);
            }
        }
    }

    /// <summary>
    /// Property type declaration of var object.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    public class VarProperty<T> : VarProperty, IVarProperty<T>
    {
        public VarProperty(Type ownerType, string propertyName, bool serializable)
            : base(ownerType, ownerType, propertyName, typeof(T), serializable) { }

        public VarProperty(Type ownerType, Type declareType, string propertyName, bool serializable)
            : base(ownerType, declareType, propertyName, typeof(T), serializable) { }
    }
}
