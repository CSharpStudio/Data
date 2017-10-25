using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.SqlTree
{
    public class SqlBinaryConstraint : SqlConstraint
    {
        public SqlBinaryConstraint()
        {
            Operator = SqlBinaryOperator.Equal;
        }

        public override SqlNodeType NodeType
        {
            get { return SqlNodeType.SqlBinaryConstraint; }
        }

        /// <summary>
        /// 第一个需要对比的列。
        /// </summary>
        public SqlNode Left { get; set; }

        /// <summary>
        /// 第二个需要对比的列。
        /// </summary>
        public SqlNode Right { get; set; }

        /// <summary>
        /// 对比条件。
        /// </summary>
        public SqlBinaryOperator Operator { get; set; }
    }
}
