using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query.Linq
{
    /// <summary>
    /// 
    /// </summary>
    class FunctionVisitor : ExpressionVisitor
    {
        Dictionary<string, ITableSource> _tables;
        EntityPropertyFinder _propertyFinder;

        Stack<List<IQueryNode>> _args = new Stack<List<IQueryNode>>();

        public IFunctionNode FunctionNode { get; set; }

        public FunctionVisitor(Dictionary<string, ITableSource> tables, EntityPropertyFinder propertyFinder)
        {
            _tables = tables;
            _propertyFinder = propertyFinder;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _propertyFinder.Find(node, _tables);
            _args.Peek().Add(_propertyFinder.PropertyOwnerTable.Column(_propertyFinder.Property.Name));
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            _args.Push(new List<IQueryNode>());
            if (!SqlFunctions.Contains(node.Method.Name))
                throw new NotSupportedException("不支持方法调用{0}".FormatArgs(node.Method.Name));
            if (node.Object != null)
                Visit(node.Object);
            foreach (var arg in node.Arguments)
                Visit(arg);
            IFunctionNode function = QueryFactory.Instance.Function(_args.Pop(), node.Method.Name);
            if (_args.Any())
                _args.Peek().Add(function);
            else
                FunctionNode = function;
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _args.Push(new List<IQueryNode>());
            Visit(node.Left);
            Visit(node.Right);
            string methodName = "";
            if (node.NodeType == ExpressionType.Add)
            {
                if (SqlFunctions.CONCAT.CIEquals(node.Method?.Name))
                    methodName = SqlFunctions.CONCAT;
                else
                    methodName = SqlBinaryFunctions.Add;
            }
            else if (node.NodeType == ExpressionType.Subtract)
                methodName = SqlBinaryFunctions.Subtract;
            else if (node.NodeType == ExpressionType.Multiply)
                methodName = SqlBinaryFunctions.Multiply;
            else if (node.NodeType == ExpressionType.Divide)
                methodName = SqlBinaryFunctions.Divide;
            else if (node.NodeType == ExpressionType.Modulo)
                methodName = SqlBinaryFunctions.Modulo;
            else throw new NotSupportedException("BinaryExpression.NodeType:" + node.NodeType);
            IFunctionNode function = null;
            var args = _args.Pop();
            if (methodName == SqlFunctions.CONCAT)
                function = QueryFactory.Instance.Function(args, methodName);
            else
                function = QueryFactory.Instance.BinaryFunction(args[0], args[1], methodName);
            if (_args.Any())
                _args.Peek().Add(function);
            else
                FunctionNode = function;
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _args.Peek().Add(QueryFactory.Instance.Value(node.Value));
            return node;
        }
    }
}
