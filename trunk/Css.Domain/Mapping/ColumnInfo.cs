using Css.Data.Common;
using Css.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Mapping
{
    public class ColumnInfo : IColumnInfo
    {
        public Type DataType { get; set; }

        public bool IsIdentity { get; set; }

        public bool IsNullable { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsTimeStamp { get; set; }

        public string Name { get; set; }

        public string PropertyName { get; set; }

        public ITableInfo Table { get; set; }

        public bool UseSequence { get; set; }

        public object GetPropertyValue(object entity)
        {
            return TypeDescriptor.GetValue(entity, PropertyName);
        }

        public void SetPropertyValue(object entity, object value)
        {
            TypeDescriptor.SetValue(entity, PropertyName, value);
        }
    }
}
