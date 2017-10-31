using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    public class HierarchicalCollection<T> : ObservableCollection<T> where T : IHierarchicalData<T>
    {
        public HierarchicalCollection()
        {
        }

        public HierarchicalCollection(List<T> list)
            : base(list)
        {

        }

        public HierarchicalCollection(IEnumerable<T> collection)
            : base(collection)
        {

        }

        public IHierarchicalData<T> Parent
        {
            get;
            set;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace
                || e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (T node in e.NewItems)
                    node.PropertyChanged += node_PropertyChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace
                || e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (T node in e.OldItems)
                    node.PropertyChanged -= node_PropertyChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //foreach (T node in e.OldItems)
                //    node.PropertyChanged -= node_PropertyChanged;
                //foreach (T node in e.NewItems)
                //    node.PropertyChanged += node_PropertyChanged;
            }
            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// 清空并重新加载集合
        /// </summary>
        /// <param name="collection"></param>
        public void Load(IEnumerable<T> collection)
        {
            Items.Clear();
            foreach (var i in collection)
            {
                i.PropertyChanged += node_PropertyChanged;
                Items.Add(i);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!supressChanged && Parent == null && e.PropertyName == "Expanded")
                OnExpandedChanged((T)sender);
        }

        void OnExpandedChanged(T node)
        {
            int index = IndexOf(node);
            if (node.Expanded)
                InsertChildren(node, ref index);
            else
                RemoveChildren(node);
        }

        int InsertChildren(T node, ref int index)
        {
            foreach (T child in node.Children)
            {
                if (!Contains(child))
                    Insert(++index, child);

                if (child.Expanded && child.Children.Count > 0)
                    InsertChildren(child, ref index);
            }
            return index;
        }

        void RemoveChildren(T node)
        {
            foreach (T child in node.Children)
            {
                if (child.Children.Count > 0)
                    RemoveChildren(child);

                Remove(child);
            }
        }

        bool supressChanged;

        public void Expand()
        {
            foreach (T item in Items.ToList())
            {
                if (!item.Expanded)
                    supressChanged = true;
                SetExpanded(item.Children, true);
                supressChanged = false;
            }
            foreach (T child in Items.ToList())
                child.Expanded = true;
        }

        void SetExpanded(IList<T> children, bool expand)
        {
            foreach (T item in children)
            {
                SetExpanded(item.Children, expand);
                item.Expanded = expand;
            }
        }

        public void Collapse()
        {
            foreach (T child in Items.ToList())
            {
                if (child.Children.Count > 0)
                    SetExpanded(child.Children, false);

                child.Expanded = false;
            }
        }
    }
}
