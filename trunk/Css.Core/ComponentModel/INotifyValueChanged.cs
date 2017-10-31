using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    /// <summary>
    /// Notifies subscribers that a property value is Changed.
    /// </summary>
    public interface INotifyValueChanged
    {
        event EventHandler<ValueChangedEventArgs> ValueChanged;
    }
}
