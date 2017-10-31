using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    public class ObservableList<T> : BindingCollection<T>, ITrackableCollection where T : ITrackable
    {
        protected List<T> _deleted = new List<T>();

        public IList Deleted { get { return _deleted; } }

        protected override void ClearItems()
        {
            _deleted.Clear();
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            item.MarkCreated();
            base.InsertItem(index, item);
        }

        void RemoveItem(T item)
        {
            if (item.DataState != DataState.Created)
            {
                item.MarkDeleted();
                _deleted.Add(item);
            }
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            RemoveItem(item);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            RemoveItem(oldItem);
            item.MarkCreated();
            base.SetItem(index, item);
        }
    }
}
