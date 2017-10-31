using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    /// <summary>
    /// Field interface of var object.
    /// </summary>
    public interface IVarField
    {
        /// <summary>
        /// Value of the field.
        /// </summary>
        object Value { get; set; }
        /// <summary>
        /// Is the field data dirty.
        /// </summary>
        bool IsDirty { get; }
        /// <summary>
        /// Name of the field.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Marks the field as unchanged.
        /// </summary>
        void MarkClean();
    }

    /// <summary>
    /// Generic type field interface of var object.
    /// </summary>
    public interface IVarField<T> : IVarField
    {
        /// <summary>
        /// Value of the field.
        /// </summary>
        new T Value { get; set; }
    }

    /// <summary>
    /// Generic type field of var object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VarField<T> : IVarField<T>
    {
        T _data;

        VarField() { }

        /// <summary>
        /// Creates a new instance of the object.
        /// </summary>
        /// <param name="name">
        /// Name of the field.
        /// </param>
        public VarField(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Is the field data dirty.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Gets the name of the field
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value of the field.
        /// </summary>
        public virtual T Value
        {
            get { return _data; }
            set
            {
                _data = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Value of the field.
        /// </summary>
        object IVarField.Value
        {
            get { return Value; }
            set
            {
                if (value == null)
                    Value = default(T);
                else
                    Value = value.ConvertTo<T>();
            }
        }
        /// <summary>
        /// Marks the field as unchanged.
        /// </summary>
        public void MarkClean()
        {
            IsDirty = false;
        }
    }
}

