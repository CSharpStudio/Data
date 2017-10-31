using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    public class ClrPropertyDescriptor : PropertyDescriptor
    {
        PropertyInfo _property;

        public ClrPropertyDescriptor(PropertyInfo property)
            : base(property.Name, null)
        {
            _property = property;
        }

        public override Type ComponentType
        {
            get { return typeof(object); }
        }

        public override bool IsReadOnly
        {
            get { return !_property.CanWrite; }
        }

        public override Type PropertyType
        {
            get { return _property.PropertyType; }
        }

        public override object GetValue(object component)
        {
            return _property.GetValue(component, null);
        }

        public override void SetValue(object component, object value)
        {
            _property.SetValue(component, value, null);
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
            throw new NotSupportedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
