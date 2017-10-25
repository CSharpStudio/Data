using System.Collections;
using System.Collections.Generic;

namespace Css.Data.SqlTree
{
    /// <summary>
    /// 表示一组结点对象
    /// 
    /// SqlNodeList 需要从 SqlConstraint 上继承，否则将不可用于 Where 语句。
    /// </summary>
    public class SqlNodeList : SqlConstraint, IEnumerable, ISqlLiteral
    {
        List<ISqlNode> _items = new List<ISqlNode>();

        public override SqlNodeType NodeType
        {
            get { return SqlNodeType.SqlNodeList; }
        }

        public void Add(ISqlNode item)
        {
            _items.Add(item);
        }

        /// <summary>
        /// 所有节点。
        /// </summary>
        public List<ISqlNode> Items
        {
            get { return _items; }
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}