using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Oracle
{
    public class OracleColumn : DbColumn
    {
        public OracleColumn(DbTable table, IColumnInfo columnInfo) : base(table, columnInfo)
        {
        }

        public override bool CanInsert
        {
            get { return true; }
        }
    }
}
