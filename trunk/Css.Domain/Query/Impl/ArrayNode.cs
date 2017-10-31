using Css.Data.SqlTree;
using System.Collections;
using System.Collections.Generic;

namespace Css.Domain.Query.Impl
{
    class ArrayNode : SqlArray, IArray
    {
        public ArrayNode() : base(false) { }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.Array; }
        }

        IList<IQueryNode> IArray.Items
        {
            get
            {
                return base.Items as IList<IQueryNode>;
            }
            set
            {
                base.Items = value as IList;
            }
        }
    }

    class AutoSelectionColumns : ArrayNode { }
}
