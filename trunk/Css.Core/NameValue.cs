using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css
{
    /// <summary>
    /// Name value pair.
    /// </summary>
    [Serializable]
    public class NameValue : NameValue<string>
    {
        /// <summary>
        /// Creates a new <see cref="NameValue"/>
        /// </summary>
        public NameValue() { }

        /// <summary>
        /// Creates a new <see cref="NameValue"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public NameValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    [Serializable]
    [DebuggerDisplay("{Name},{Value}")]
    public class NameValue<T> : ObservableObject
    {
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        public T Value
        {
            get { return Get<T>(); }
            set { Set(value); }
        }

        /// <summary>
        /// Creates a new <see cref="NameValue"/>
        /// </summary>
        public NameValue() { }

        /// <summary>
        /// Creates a new <see cref="NameValue"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214")]
        public NameValue(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
