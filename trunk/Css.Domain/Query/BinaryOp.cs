using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    /// <summary>
    /// 属性的对比操作符
    /// </summary>
    public enum BinaryOp
    {
        Equal = SqlBinaryOperator.Equal,
        NotEqual = SqlBinaryOperator.NotEqual,
        Greater = SqlBinaryOperator.Greater,
        GreaterEqual = SqlBinaryOperator.GreaterEqual,
        Less = SqlBinaryOperator.Less,
        LessEqual = SqlBinaryOperator.LessEqual,

        Like = SqlBinaryOperator.Like,
        NotLike = SqlBinaryOperator.NotLike,
        Contains = SqlBinaryOperator.Contains,
        NotContains = SqlBinaryOperator.NotContains,
        StartsWith = SqlBinaryOperator.StartsWith,
        NotStartsWith = SqlBinaryOperator.NotStartsWith,
        EndsWith = SqlBinaryOperator.EndsWith,
        NotEndsWith = SqlBinaryOperator.NotEndsWith,

        In = SqlBinaryOperator.In,
        NotIn = SqlBinaryOperator.NotIn,
    }

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

