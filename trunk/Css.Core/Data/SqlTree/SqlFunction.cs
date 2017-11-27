using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.SqlTree
{
    public class SqlFunction : SqlNode
    {
        public override SqlNodeType NodeType
        {
            get { return SqlNodeType.SqlFunction; }
        }

        public SqlArray Args { get; set; }

        /// <summary>
        /// 别名。
        /// 列的别名只用在 Select 语句之后。
        /// </summary>
        public string Alias { get; set; }

        public string Function { get; set; }
    }
}
