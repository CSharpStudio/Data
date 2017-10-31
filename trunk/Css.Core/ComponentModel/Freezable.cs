using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    public class Freezable : ICloneable
    {
        Hashtable values = new Hashtable();

        public bool IsFrozen { get; set; }

        public void Freeze()
        {
            IsFrozen = true;
        }

        protected void CheckFrozen()
        {
            if (IsFrozen)
                throw new InvalidOperationException("Current status is frozen, cannot set the value.");
        }

        protected T Get<T>([CallerMemberName]string propertyName = null)
        {
            if (values.ContainsKey(propertyName))
                return values[propertyName].ConvertTo<T>();
            return default(T);
        }

        protected void Set<T>(T value, [CallerMemberName]string propertyName = null)
        {
            CheckFrozen();
            values[propertyName] = value;
        }

        public void CopyTo(Freezable target)
        {
            foreach (DictionaryEntry v in values)
            {
                var value = v.Value;
                if (value is ICloneable)
                    value = ((ICloneable)value).Clone();
                target.values[v.Key] = values;
            }
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns></returns>
        protected virtual object Clone()
        {
            var type = GetType();
            var m = (Freezable)Activator.CreateInstance(type);
            CopyTo(m);
            return m;
        }

        /// <summary>
        /// Implement the ICloneable interface. 
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
