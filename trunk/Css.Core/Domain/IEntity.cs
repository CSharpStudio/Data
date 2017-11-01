using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public interface IEntity : IDomain, IDataErrorInfo, ICloneable, INotifyPropertyChanged, ITrackable
    {
        /// <summary>
        /// Get current entity id.
        /// </summary>
        /// <returns></returns>
        object GetId();

        /// <summary>
        /// Set current entity id.
        /// </summary>
        /// <param name="id"></param>
        void SetId(object id);

        /// <summary>
        /// Generate an identity value for this entity.
        /// </summary>
        /// <returns></returns>
        void GenerateId();
    }
}
