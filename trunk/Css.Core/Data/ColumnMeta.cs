using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data
{
    public class ColumnMeta
    {
        public bool HasFKConstraint { get; set; }

        public bool IsIdentity { get; set; }

        public bool UseSequence { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool? IsRequired { get; set; }

        public bool IsTimeStamp { get; set; }

        public string ColumnName { get; set; }

        public DbType? DataType { get; set; }

        public string DataTypeLength { get; set; }
    }
}
