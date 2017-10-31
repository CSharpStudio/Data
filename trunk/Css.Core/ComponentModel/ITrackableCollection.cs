using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    public interface ITrackableCollection : IList
    {
        IList Deleted { get; }
        void ResumeNotifyChanged();
        void SuppressNotifyChanged();
        void RaiseCollectionChanged();
    }
}
