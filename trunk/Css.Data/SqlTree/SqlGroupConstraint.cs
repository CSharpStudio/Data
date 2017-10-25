using System;

namespace Css.Data.SqlTree
{
    /// <summary>
    /// 表示作用于两个操作结点的二位运算结点。
    /// </summary>
    public class SqlGroupConstraint : SqlConstraint
    {
        ISqlConstraint _left, _right;

        public override SqlNodeType NodeType
        {
            get { return SqlNodeType.SqlGroupConstraint; }
        }

        /// <summary>
        /// 二位运算的左操作节点。
        /// </summary>
        public ISqlConstraint Left
        {
            get { return _left; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _left = value;
            }
        }

        /// <summary>
        /// 二位运算类型
        /// </summary>
        public SqlGroupOperator Opeartor { get; set; }

        /// <summary>
        /// 二位运算的右操作节点。
        /// </summary>
        public ISqlConstraint Right
        {
            get { return _right; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _right = value;
            }
        }
    }
}