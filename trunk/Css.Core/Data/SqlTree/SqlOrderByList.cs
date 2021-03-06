﻿using System.Collections;
using System.Collections.Generic;

namespace Css.Data.SqlTree
{
    /// <summary>
    /// 表示一组排序条件。
    /// </summary>
    public class SqlOrderByList : SqlNode, IEnumerable
    {
        IList _items;

        public override SqlNodeType NodeType
        {
            get { return SqlNodeType.SqlOrderByList; }
        }

        public int Count
        {
            get
            {
                if (_items == null) { return 0; }

                return _items.Count;
            }
        }

        public IList Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<SqlOrderBy>();
                }
                return _items;
            }
            set { _items = value; }
        }

        public void Add(object item)
        {
            Items.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
