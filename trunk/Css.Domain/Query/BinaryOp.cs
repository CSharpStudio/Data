using Css.Data;
using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    internal class BinaryOperatorHelper
    {
        public static BinaryOp Reverse(BinaryOp op)
        {
            switch (op)
            {
                case BinaryOp.Equal:
                    return BinaryOp.NotEqual;
                case BinaryOp.NotEqual:
                    return BinaryOp.Equal;
                case BinaryOp.Greater:
                    return BinaryOp.LessEqual;
                case BinaryOp.GreaterEqual:
                    return BinaryOp.Less;
                case BinaryOp.Less:
                    return BinaryOp.GreaterEqual;
                case BinaryOp.LessEqual:
                    return BinaryOp.Greater;
                case BinaryOp.Like:
                    return BinaryOp.NotLike;
                case BinaryOp.NotLike:
                    return BinaryOp.Like;
                case BinaryOp.Contains:
                    return BinaryOp.NotContains;
                case BinaryOp.NotContains:
                    return BinaryOp.Contains;
                case BinaryOp.StartsWith:
                    return BinaryOp.NotStartsWith;
                case BinaryOp.NotStartsWith:
                    return BinaryOp.StartsWith;
                case BinaryOp.EndsWith:
                    return BinaryOp.NotEndsWith;
                case BinaryOp.NotEndsWith:
                    return BinaryOp.EndsWith;
                case BinaryOp.In:
                    return BinaryOp.NotIn;
                case BinaryOp.NotIn:
                    return BinaryOp.In;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

