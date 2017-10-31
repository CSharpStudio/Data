using Css.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query.Linq
{
    class LiteralVisitor : ExpressionVisitor
    {
        public const string MethodName = "SQL";

        public ILiteral Literal { get; set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Arguments.Count != 2)
                throw new ArgumentException("方法{0}参数个数不对".FormatArgs(node.Method.Name));
            var sql = (node.Arguments[1] as ConstantExpression)?.Value as FormattedSql;
            if (sql == null)
                throw new InvalidCastException("参数无法转为FormattedSql");
            Literal = QueryFactory.Instance.Literal(sql.ToString(), sql.Parameters);
            return node;
        }
    }
}
