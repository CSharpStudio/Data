using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    public class VarPropertyDescriptor : PropertyDescriptor
    {
        VarProperty _property;

        public VarPropertyDescriptor(VarProperty property)
            : base(property.Name, null)
        {
            _property = property;
        }

        public VarProperty Property
        {
            get { return _property; }
        }

        public override Type ComponentType
        {
            get { return _property.OwnerType; }
        }

        public override bool IsReadOnly
        {
            get { return _property.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return _property.PropertyType; }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return (component as VarObject)[_property];
        }

        public override void SetValue(object component, object value)
        {
            (component as VarObject)[_property] = value;
        }

        public override void ResetValue(object component)
        {
            throw new NotSupportedException();
        }
    }
}
