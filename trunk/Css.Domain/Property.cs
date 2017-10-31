using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class Property<TPropertyType> : VarProperty<TPropertyType>, IProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Property{TPropertyType}"/> class.
        /// </summary>
        /// <param name="ownerType">Type of the owner.</param>
        /// <param name="declareType">Type of the declare.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="defaultMeta">The default meta.</param>
        public Property(Type ownerType, Type declareType, string propertyName, bool serializable) : base(ownerType, declareType, propertyName, serializable) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property{TPropertyType}"/> class.
        /// </summary>
        /// <param name="ownerType">Type of the owner.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="defaultMeta">The default meta.</param>
        public Property(Type ownerType, string propertyName, bool serializable) : base(ownerType, propertyName, serializable) { }

        /// <summary>
        /// SIE 属性的类型
        /// </summary>
        public virtual PropertyCategory Category
        {
            get
            {
                if (IsReadOnly) { return PropertyCategory.Readonly; }
                return PropertyCategory.Normal;
            }
        }
    }
}
