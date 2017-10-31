using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    public class HierarchicalData<T> : ObservableObject, IHierarchicalData<T> where T : IHierarchicalData<T>
    {
        public HierarchicalData()
        {
            Children = new BindingCollection<T>();
        }

        public virtual T Parent
        {
            get { return Get<T>(); }
            set { Set(value); }
        }

        public virtual BindingCollection<T> Children { get; private set; }

        public virtual bool Expanded
        {
            get { return Get<bool>(); }
            set { Set(value); }
        }

        public virtual int Level
        {
            get
            {
                int result = 1;
                var parent = Parent;
                while (parent != null)
                {
                    result++;
                    parent = parent.Parent;
                }

                return result;
            }
        }

        public virtual bool HasChild
        {
            get { return Children.Count > 0; }
        }
    }
}
