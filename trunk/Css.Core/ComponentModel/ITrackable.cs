using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    public interface ITrackable
    {
        event EventHandler StateChanged;
        /// <summary>
        /// 状态
        /// </summary>
        DataState DataState { get; }

        /// <summary>
        /// Change the DataState to Created.
        /// </summary>
        void MarkCreated();

        /// <summary>
        /// Change the DataState to Modified.
        /// </summary>
        void MarkModified();

        /// <summary>
        /// Change the DataState to Deleted.
        /// </summary>
        void MarkDeleted();

        /// <summary>
        /// Change the DataState to None.
        /// </summary>
        void ResetState();

        void SuppressNotifyChanged();

        void ResumeNotifyChanged();

        void RaisePropertyChanged(string propertyName);

        void Set<T>(T value, string propertyName);
    }
}
