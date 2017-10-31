using Css.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    /// <summary>
    /// Value initialize delegate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public delegate T ValueInitializer<T>();

    /// <summary>
    /// Observable object.
    /// </summary>
    [Serializable]
    [System.Runtime.Serialization.DataContract]
    public class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging, INotifyValueChanged, ICloneable
    {
        Hashtable valueTable = new Hashtable();

        [NonSerialized]
        PropertyChangedEventHandler propertyChanged;
        [NonSerialized]
        PropertyChangingEventHandler propertyChanging;
        [NonSerialized]
        EventHandler<ValueChangedEventArgs> valueChanged;
        [NonSerialized]
        bool suppressPropertyChanged = false;

        /// <summary>
        /// The value storage table.
        /// </summary>
        protected internal virtual Hashtable ValueTable
        {
            get { return valueTable; }
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged
        {
            add { valueChanged = (EventHandler<ValueChangedEventArgs>)System.Delegate.Combine(valueChanged, value); }
            remove { valueChanged = (EventHandler<ValueChangedEventArgs>)System.Delegate.Remove(valueChanged, value); }
        }

        public event PropertyChangingEventHandler PropertyChanging
        {
            add { propertyChanging = (PropertyChangingEventHandler)System.Delegate.Combine(propertyChanging, value); }
            remove { propertyChanging = (PropertyChangingEventHandler)System.Delegate.Remove(propertyChanging, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged = (PropertyChangedEventHandler)System.Delegate.Combine(propertyChanged, value); }
            remove { propertyChanged = (PropertyChangedEventHandler)System.Delegate.Remove(propertyChanged, value); }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (!suppressPropertyChanged && propertyChanging != null)
                propertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual internal void OnPropertyChanged(string propertyName)
        {
            if (!suppressPropertyChanged && propertyChanged != null)
            {
                propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                propertyChanged.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnValueChanged(string propertyName,
            object newValue, object oldValue)
        {
            if (!suppressPropertyChanged && valueChanged != null)
                valueChanged.Invoke(this, new ValueChangedEventArgs(propertyName, newValue, oldValue));
        }
        /// <summary>
        /// 屬性索引器，可用於存取額外的屬性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual object this[string propertyName]
        {
            get { return Get<object>(property: propertyName); }
            set { Set(value, propertyName); }
        }

        /// <summary>
        /// Get property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public virtual T Get<T>(ValueInitializer<T> initializer = null, [CallerMemberName] string property = null)
        {
            if (valueTable.ContainsKey(property))
                return (T)valueTable[property];
            if (initializer != null)
            {
                var value = initializer.Invoke();
                valueTable[property] = value;
                return value;
            }
            return default(T);
        }

        /// <summary>
        /// Set property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public virtual bool Set<T>(T newValue, [CallerMemberName]string property = null)
        {
            T oldValue = Get<T>(property: property);

            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return false;

            OnPropertyChanging(property);
            valueTable[property] = newValue;
            OnValueChanged(property, newValue, oldValue);
            OnPropertyChanged(property);
            return true;
        }

        /// <summary>
        /// 拷贝数据到目标，只触发一次通知事件。
        /// <para>目標可以是當前類型的父類或者子類</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <exception cref="ArgumentException">当前类型与目标类型不一致</exception>
        public virtual void CopyTo<T>(T destination) where T : ObservableObject
        {
            if (this == destination) return;
            Type sourceType = GetType();
            Type targetType = typeof(T);
            if (!sourceType.Equals(targetType) && !(this is T) && !targetType.IsSubclassOf(sourceType))
                throw new ArgumentException("当前类型{0}与目标类型{1}不一致".FormatArgs(sourceType, targetType));
            destination.valueTable.Clear();
            foreach (DictionaryEntry e in valueTable)
                destination.valueTable.Add(e.Key, ObjectCloner.Clone(e.Value));
            destination.OnPropertyChanged("");
        }
        /// <summary>
        /// Raise the property changed event.
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
        /// <summary>
        /// Resume the property changed event.
        /// </summary>
        public virtual void ResumePropertyChanged()
        {
            suppressPropertyChanged = false;
        }
        /// <summary>
        /// Suppress the property changed event.
        /// </summary>
        public virtual void SuppressPropertyChanged()
        {
            suppressPropertyChanged = true;
        }

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
            var m = (ObservableObject)Activator.CreateInstance(type);
            CopyTo(m);
            return m;
        }
    }
}
