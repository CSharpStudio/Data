using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    /// <summary>
    /// 可绑定集合，继承自<see cref="ObservableCollection<T>"/>, 实现<see cref="IBindingList"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindingCollection<T> : ObservableCollection<T>, IBindingList
    {
        public BindingCollection() { }
        public BindingCollection(IEnumerable<T> collection) : base(collection) { }
        public BindingCollection(List<T> list) : base(list) { }

        public event ListChangedEventHandler ListChanged;

        [NonSerialized]
        PropertyDescriptor m_PropertyDescriptor;
        [NonSerialized]
        ListSortDirection m_ListSortDirection;

        [NonSerialized]
        bool _suppressNotifyChanged = false;

        protected override void ClearItems()
        {
            base.ClearItems();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            base.MoveItem(oldIndex, newIndex);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, newIndex, oldIndex));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotifyChanged)
                base.OnCollectionChanged(e);
        }

        public void RaiseCollectionChanged()
        {
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        /// <summary>
        /// 清空后把数据加载到集合中，加载完成后触发CollectionChanged事件
        /// </summary>
        /// <param name="source"></param>
        public virtual void Load(IEnumerable<T> source)
        {
            Items.Clear();
            foreach (var item in source)
                Items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        }

        /// <summary>
        /// 把数据加载到集合中，加载完成后触发CollectionChanged事件
        /// </summary>
        /// <param name="source"></param>
        public void AddRange(IEnumerable<T> source)
        {
            foreach (var item in source)
                Items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        }

        public virtual void ResumeNotifyChanged()
        {
            _suppressNotifyChanged = false;
        }

        public virtual void SuppressNotifyChanged()
        {
            _suppressNotifyChanged = true;
        }

        public string Sort
        {
            get
            {
                if (m_PropertyDescriptor != null)
                    return m_PropertyDescriptor.Name + (m_ListSortDirection == ListSortDirection.Descending ? " DESC" : " ASC");
                return null;
            }
        }

        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            ListChanged?.Invoke(this, e);
        }

        #region IBindingList

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        void IBindingList.AddIndex(PropertyDescriptor property)
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, property));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object IBindingList.AddNew()
        {
            var t = typeof(T);
            if (!t.IsAbstract && !t.IsInterface)
                return Activator.CreateInstance(t);
            return default(T);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        bool IBindingList.AllowEdit
        {
            get { return true; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        bool IBindingList.AllowNew
        {
            get { return true; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        bool IBindingList.AllowRemove
        {
            get { return true; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            bool isChanged = m_PropertyDescriptor != property || m_ListSortDirection != direction;
            m_PropertyDescriptor = property;
            m_ListSortDirection = direction;
            if (isChanged)
                OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, property));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            foreach (var item in this)
            {
                if (item is VarObject)
                {
                    if (object.Equals((item as VarObject)[property.Name], key))
                        return IndexOf(item);
                }
                if (item is ObservableObject)
                {
                    if (object.Equals((item as ObservableObject)[property.Name], key))
                        return IndexOf(item);
                }
                else
                {
                    if (object.Equals(property.GetValue(item), key))
                        return IndexOf(item);
                }
            }
            return -1;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        bool IBindingList.IsSorted
        {
            get { return m_PropertyDescriptor != null; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        event ListChangedEventHandler IBindingList.ListChanged
        {
            add { ListChanged += value; }
            remove { ListChanged -= value; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, property));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        void IBindingList.RemoveSort()
        {
            m_PropertyDescriptor = null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        ListSortDirection IBindingList.SortDirection
        {
            get { return m_ListSortDirection; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        PropertyDescriptor IBindingList.SortProperty
        {
            get { return m_PropertyDescriptor; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        bool IBindingList.SupportsChangeNotification
        {
            get { return true; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        bool IBindingList.SupportsSearching
        {
            get { return true; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        bool IBindingList.SupportsSorting
        {
            get { return true; }
        }

        #endregion
    }
}

