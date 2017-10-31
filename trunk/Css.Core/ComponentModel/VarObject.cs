using Css.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    [Serializable]
    public class VarObject : ICustomTypeDescriptor, INotifyPropertyChanged, INotifyValueChanged, ICloneable
    {
        VarFieldData _fieldData;

        protected VarFieldData FieldData { get { return _fieldData; } }

        [NonSerialized]
        PropertyChangedEventHandler _propertyChangedHandler;
        [NonSerialized]
        EventHandler<ValueChangedEventArgs> _valueChangedHandler;
        [NonSerialized]
        bool _suppressNotifyChanged;

        /// <summary>
        /// 属性变更后事件。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChangedHandler = (PropertyChangedEventHandler)Delegate.Combine(_propertyChangedHandler, value); }
            remove { _propertyChangedHandler = (PropertyChangedEventHandler)Delegate.Remove(_propertyChangedHandler, value); }
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged
        {
            add { _valueChangedHandler = (EventHandler<ValueChangedEventArgs>)Delegate.Combine(_valueChangedHandler, value); }
            remove { _valueChangedHandler = (EventHandler<ValueChangedEventArgs>)Delegate.Remove(_valueChangedHandler, value); }
        }

        public VarPropertyContainer PropertyContainer { get; }

        public VarObject()
        {
            PropertyContainer = VarTypeRepository.Instance.GetVarPropertyContainer(GetType());
            _fieldData = new VarFieldData(PropertyContainer);
        }

        public IVarProperty GetProperty(string propertyName)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            return DoGetProperty(propertyName);
        }

        public virtual T Get<T>(IVarProperty<T> property)
        {
            Check.NotNull(property, nameof(property));
            return GetValue<T>(property);
        }

        public virtual T Get<T>([CallerMemberName]string propertyName = null)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            var property = DoGetProperty(propertyName);
            return GetValue<T>(property);
        }

        public virtual void Set<T>(T value, IVarProperty<T> property)
        {
            Check.NotNull(property, nameof(property));
            SetValue(value, property);
        }

        public virtual void Set<T>(T value, [CallerMemberName]string propertyName = null)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            var property = DoGetProperty(propertyName);
            SetValue(value, property);
        }

        public virtual object this[string propertyName]
        {
            get
            {
                Check.NotNullOrEmpty(propertyName, nameof(propertyName));
                var property = DoGetProperty(propertyName);
                return GetValue(property);
            }
            set
            {
                Check.NotNullOrEmpty(propertyName, nameof(propertyName));
                var property = DoGetProperty(propertyName);
                SetValue(value, property);
            }
        }

        public virtual object this[IVarProperty property]
        {
            get
            {
                Check.NotNull(property, nameof(property));
                return GetValue(property);
            }
            set
            {
                Check.NotNull(property, nameof(property));
                SetValue(value, property);
            }
        }

        public void LoadValue(object value, IVarProperty property)
        {
            Check.NotNull(property, nameof(property));
            CheckPropertyWritable(property);
            object oldValue = GetValue(property);
            _fieldData.LoadField(property, value);
            RasiePropertyValueChanged(property, oldValue, value);
        }

        protected virtual IVarProperty DoGetProperty(string propertyName)
        {
            var property = PropertyContainer.FindProperty(propertyName);
            if (property == null)
                throw new ArgumentOutOfRangeException("类型[{0}]未定义属性[{1}]".FormatArgs(GetType().GetQualifiedName(), propertyName));
            return property;
        }

        protected virtual T GetValue<T>(IVarProperty property)
        {
            if (property.IsReadOnly)
                return property.GetReadOnlyValue(this).ConvertTo<T>();
            var field = _fieldData.GetField(property);
            T result;
            if (field != null)
            {
                var fd = field as IVarField<T>;
                if (fd != null)
                    result = fd.Value;
                else
                    result = field.Value.ConvertTo<T>();
            }
            else
            {
                result = default(T);
            }
            return result;
        }

        protected virtual void SetValue<T>(T value, IVarProperty property)
        {
            CheckPropertyWritable(property);
            var oldValue = GetValue<T>(property);
            if (Comparer<T>.Default.Compare(oldValue, value) != 0)
            {
                _fieldData.SetField(property, value);
                RasiePropertyValueChanged(property, oldValue, value);
            }
        }

        protected virtual object GetValue(IVarProperty property)
        {
            if (property.IsReadOnly)
                return property.GetReadOnlyValue(this);
            var field = _fieldData.GetField(property);
            if (field != null)
                return field.Value;
            return property.PropertyType.GetDefault();
        }

        protected virtual void SetValue(VarProperty property, object value)
        {
            CheckPropertyWritable(property);
            var oldValue = GetValue(property);
            if (!object.Equals(oldValue, value))
            {
                _fieldData.SetField(property, value);
                RasiePropertyValueChanged(property, oldValue, value);
            }
        }

        void CheckPropertyWritable(IVarProperty property)
        {
            if (property.IsReadOnly) throw new InvalidOperationException("类型[{0}]属性[{1}]是只读的".FormatArgs(property.OwnerType.Name, property.Name));
        }

        protected virtual void RasiePropertyValueChanged(IVarProperty property, object oldValue, object newValue)
        {
            if (_suppressNotifyChanged) return;
            OnPropertyChanged("Item[]");//触发索引器属性变更通知
            OnPropertyChanged(property.Name);
            OnValueChanged(property.Name, oldValue, newValue);
            foreach (var d in property.ReadonlyDependencies)
                OnPropertyChanged(d.Name);
        }

        protected virtual void OnValueChanged(string propertyName, object oldValue, object newValue)
        {
            if (_suppressNotifyChanged) return;
            _valueChangedHandler?.Invoke(this, new ValueChangedEventArgs(propertyName, oldValue, newValue));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (_suppressNotifyChanged) return;
            _propertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 觸發屬性變更通知
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual void RaisePropertyChanged(string propertyName)
        {
            _suppressNotifyChanged = true;
            OnPropertyChanged(propertyName);
        }
        /// <summary>
        /// 恢復屬性變更通知
        /// </summary>
        public virtual void ResumeNotifyChanged()
        {
            _suppressNotifyChanged = false;
        }
        /// <summary>
        /// 禁用屬性變更通知
        /// </summary>
        public virtual void SuppressNotifyChanged()
        {
            _suppressNotifyChanged = true;
        }

        #region ICustomTypeDescriptor Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return PropertyContainer.GetPropertyDescriptorCollection();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return AttributeCollection.Empty;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        string ICustomTypeDescriptor.GetClassName()
        {
            return null;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        #region Clone/CopyTo

        /// <summary>
        /// Implement the ICloneable interface. 
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns></returns>
        protected virtual object Clone()
        {
            var type = GetType();
            var m = (VarObject)Activator.CreateInstance(type);
            CopyTo(m);
            return m;
        }

        /// <summary>
        /// 拷贝数据到目标，只触发一次通知事件。
        /// <para>目標可以是當前類型的父類或者子類</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <exception cref="ArgumentException">当前类型与目标类型不一致</exception>
        public virtual void CopyTo<T>(T destination) where T : VarObject
        {
            destination._fieldData = (VarFieldData)ObjectCloner.Clone(FieldData);
            destination.OnPropertyChanged("");
        }

        #endregion

        #region ForDebuggerDisplay

        List<PropertyValue> PropertyValues
        {
            get
            {
                var result = new List<PropertyValue>();
                PropertyContainer.Properties.ForEach(p => result.Add(new PropertyValue { Property = p, Value = GetValue(p) }));
                return result;
            }
        }

        [DebuggerDisplay("{Property.Name}: {Value}")]
        class PropertyValue
        {
            public VarProperty Property { get; set; }
            public object Value { get; set; }
        }

        #endregion
    }
}
