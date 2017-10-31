using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    public interface IVarProperty
    {
        string Name { get; }
        Type OwnerType { get; }
        Type DeclareType { get; }
        Type PropertyType { get; }
        bool IsReadOnly { get; }
        object GetReadOnlyValue(VarObject obj);
        int TypeCompiledIndex { get; }
        int GlobalIndex { get; }
        IList<VarPropertyBase> ReadonlyDependencies { get; }
        bool Serializable { get; }
    }
    public interface IVarProperty<T> : IVarProperty { }

    /// <summary>
    /// Property type declaration of var object.
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
        
        public bool Serializable { get; set; }

        protected internal abstract object GetReadOnlyValue(VarObject obj);

        object IVarProperty.GetReadOnlyValue(VarObject obj)
        {
            return GetReadOnlyValue(obj);
        }

        internal int TypeCompiledIndex { get; set; }

        internal int GlobalIndex { get; set; }

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
            if (ownerType == null) throw new ArgumentNullException("ownerType");
            if (declareType == null) throw new ArgumentNullException("declareType");
            if (propertyName.IsNullOrEmpty()) throw new ArgumentNullException("propertyName");

            TypeCompiledIndex = -1;
            GlobalIndex = -1;
            OwnerType = ownerType;
            DeclareType = declareType;
            Serializable = serializable;
            PropertyType = propertyType;
            Name = propertyName;
        }

        public void AsReadOnly(Func<VarObject, object> readOnlyValueProvider, params VarPropertyBase[] dependencies)
        {
            if (GlobalIndex >= 0) throw new InvalidOperationException("属性已经注册完毕，不能修改！");

            if (readOnlyValueProvider == null) throw new ArgumentNullException("readOnlyValueProvider");

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
