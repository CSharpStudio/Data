using Css.Data;
using Css.Data.Common;
using Css.Data.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query.Linq
{
    public class CriteriaBuilder : IClientCriteriaVisitor
    {
        IQuery _query;
        QueryFactory f = QueryFactory.Instance;
        IList<IQuery> queries = new List<IQuery>();

        public CriteriaBuilder(IQuery query)
        {
            _query = query;
            queries.Add(query);
        }

        public IConstraint Build(CriteriaOperator criteria)
        {
            return (IConstraint)criteria.Accept(this);
        }

        [System.Diagnostics.DebuggerStepThrough]
        object Process(CriteriaOperator criteria)
        {
            return criteria.Accept(this);
        }

        public object Visit(BetweenOperator theOperator)
        {
            var property = (IQueryNode)Process(theOperator.TestExpression);
            var from = (IQueryNode)Process(theOperator.BeginExpression);
            var to = (IQueryNode)Process(theOperator.EndExpression);
            var begin = f.Constraint(property, BinaryOp.GreaterEqual, from);
            var end = f.Constraint(property, BinaryOp.LessEqual, to);
            return f.And(begin, end);
        }

        public object Visit(BinaryOperator theOperator)
        {
            var left = (IQueryNode)Process(theOperator.LeftOperand);
            var right = (IQueryNode)Process(theOperator.RightOperand);
            switch (theOperator.OperatorType)
            {
                case BinaryOperatorType.Equal:
                case BinaryOperatorType.Greater:
                case BinaryOperatorType.GreaterOrEqual:
                case BinaryOperatorType.Less:
                case BinaryOperatorType.LessOrEqual:
                case BinaryOperatorType.Like:
                case BinaryOperatorType.NotEqual:
                    return f.Constraint(left, GetOp(theOperator.OperatorType), right);
                case BinaryOperatorType.Divide:
                    return f.BinaryFunction(left, right, SqlBinaryFunctions.Divide);
                case BinaryOperatorType.Minus:
                    return f.BinaryFunction(left, right, SqlBinaryFunctions.Subtract);
                case BinaryOperatorType.Modulo:
                    return f.BinaryFunction(left, right, SqlBinaryFunctions.Modulo);
                case BinaryOperatorType.Multiply:
                    return f.BinaryFunction(left, right, SqlBinaryFunctions.Multiply);
                case BinaryOperatorType.Plus:
                    return f.BinaryFunction(left, right, SqlBinaryFunctions.Add);
            }
            throw new NotSupportedException("[{0}]".FormatArgs(theOperator.OperatorType));
        }

        BinaryOp GetOp(BinaryOperatorType type)
        {
            switch (type)
            {
                case BinaryOperatorType.Equal: return BinaryOp.Equal;
                case BinaryOperatorType.Greater: return BinaryOp.Greater;
                case BinaryOperatorType.GreaterOrEqual: return BinaryOp.GreaterEqual;
                case BinaryOperatorType.Less: return BinaryOp.Less;
                case BinaryOperatorType.LessOrEqual: return BinaryOp.LessEqual;
                case BinaryOperatorType.Like: return BinaryOp.Contains;
                case BinaryOperatorType.NotEqual: return BinaryOp.NotEqual;
            }
            return BinaryOp.Contains;
        }

        public object Visit(UnaryOperator theOperator)
        {
            var unary = (IQueryNode)Process(theOperator.Operand);
            if (theOperator.OperatorType == UnaryOperatorType.Not)
            {
                if (unary is IConstraint)
                    return f.Not((IConstraint)unary);
                return f.Not(f.Constraint(unary, true));
            }
            if (theOperator.OperatorType == UnaryOperatorType.Minus)
                return f.BinaryFunction(f.Value(0), unary, SqlBinaryFunctions.Subtract);
            if (theOperator.OperatorType == UnaryOperatorType.Plus)
                return f.BinaryFunction(f.Value(0), unary, SqlBinaryFunctions.Add);
            if (theOperator.OperatorType == UnaryOperatorType.IsNull)
                return f.Constraint(unary, null);
            throw new NotSupportedException(theOperator.LegacyToString());
        }

        public object Visit(InOperator theOperator)
        {
            var left = (IQueryNode)Process(theOperator.LeftOperand);
            var array = new List<IQueryNode>();
            foreach (var op in theOperator.Operands)
                array.Add((IQueryNode)Process(op));
            return f.Constraint(left, BinaryOp.In, f.Array(array.ToArray()));
        }

        public object Visit(GroupOperator theOperator)
        {
            var constraints = new List<IConstraint>(theOperator.Operands.Count);
            foreach (var op in theOperator.Operands)
                constraints.Add(Process(op) as IConstraint);
            if (theOperator.OperatorType == GroupOperatorType.And)
                return f.And(constraints.ToArray());
            else
                return f.Or(constraints.ToArray());
        }

        public object Visit(OperandValue theOperand)
        {
            return f.Value(theOperand.Value);
        }

        public object Visit(FunctionOperator theOperator)
        {
            List<IQueryNode> nodes = new List<IQueryNode>(theOperator.Operands.Count);
            foreach (var op in theOperator.Operands)
                nodes.Add((IQueryNode)Process(op));
            if (theOperator.OperatorType == FunctionOperatorType.Abs)
                return f.Function(nodes, SqlFunctions.ABS);
            if (theOperator.OperatorType == FunctionOperatorType.Acos)
                return f.Function(nodes, SqlFunctions.ACOS);
            if (theOperator.OperatorType == FunctionOperatorType.Asin)
                return f.Function(nodes, SqlFunctions.ASIN);
            if (theOperator.OperatorType == FunctionOperatorType.Atn)
                return f.Function(nodes, SqlFunctions.ATAN);
            if (theOperator.OperatorType == FunctionOperatorType.Ceiling)
                return f.Function(nodes, SqlFunctions.CEILING);
            if (theOperator.OperatorType == FunctionOperatorType.Concat)
                return f.Function(nodes, SqlFunctions.CONCAT);
            if (theOperator.OperatorType == FunctionOperatorType.Cos)
                return f.Function(nodes, SqlFunctions.COS);
            if (theOperator.OperatorType == FunctionOperatorType.GetDate)
                return f.Function(nodes, SqlFunctions.DAY);
            if (theOperator.OperatorType == FunctionOperatorType.Exp)
                return f.Function(nodes, SqlFunctions.EXP);
            if (theOperator.OperatorType == FunctionOperatorType.Floor)
                return f.Function(nodes, SqlFunctions.FLOOR);
            if (theOperator.OperatorType == FunctionOperatorType.Len)
                return f.Function(nodes, SqlFunctions.LENGTH);
            if (theOperator.OperatorType == FunctionOperatorType.Lower)
                return f.Function(nodes, SqlFunctions.LOWER);
            if (theOperator.OperatorType == FunctionOperatorType.Trim)
                return f.Function(f.Function(nodes, SqlFunctions.LTRIM), SqlFunctions.RTRIM);
            if (theOperator.OperatorType == FunctionOperatorType.GetMinute)
                return f.Function(nodes, SqlFunctions.MONTH);
            if (theOperator.OperatorType == FunctionOperatorType.IsNull)
                return f.Function(nodes, SqlFunctions.NVL);
            if (theOperator.OperatorType == FunctionOperatorType.Power)
                return f.Function(nodes, SqlFunctions.POWER);
            if (theOperator.OperatorType == FunctionOperatorType.Round)
                return f.Function(nodes, SqlFunctions.ROUND);
            if (theOperator.OperatorType == FunctionOperatorType.Sign)
                return f.Function(nodes, SqlFunctions.SIGN);
            if (theOperator.OperatorType == FunctionOperatorType.Sin)
                return f.Function(nodes, SqlFunctions.SIN);
            if (theOperator.OperatorType == FunctionOperatorType.Sqr)
                return f.Function(nodes, SqlFunctions.SQRT);
            if (theOperator.OperatorType == FunctionOperatorType.Substring)
                return f.Function(nodes, SqlFunctions.SUBSTR);
            if (theOperator.OperatorType == FunctionOperatorType.Tan)
                return f.Function(nodes, SqlFunctions.TAN);
            if (theOperator.OperatorType == FunctionOperatorType.Upper)
                return f.Function(nodes, SqlFunctions.UPPER);
            if (theOperator.OperatorType == FunctionOperatorType.GetYear)
                return f.Function(nodes, SqlFunctions.YEAR);
            throw new NotImplementedException();
        }

        public object Visit(AggregateOperand theOperand)
        {
            if (theOperand.AggregateType == Aggregate.Exists)
            {
                var name = (theOperand.CollectionProperty as OperandProperty).PropertyName;
                var alias = name.Substring(0, Math.Max(0, name.LastIndexOf(':')));
                var property = name.Substring(name.LastIndexOf(':') + 1);
                var ownerTable = _lastJoinTable ?? _query.MainTable;
                var listProperty = ownerTable.EntityRepository.FindProperty(property) as IListProperty;
                if (listProperty == null)
                    throw new QueryException("属性{0}不是IListProperty类型,不能聚合查询".FormatArgs(property));
                var childRepo = RF.Find(listProperty.EntityType);
                var childTable = f.Table(childRepo);
                var subQuery = f.Query(from: childTable, selection: f.Literal("1"));
                queries.Add(subQuery);
                if (alias.IsNotEmpty())
                    childTable.Alias = alias;
                else
                {
                    var qgc = QueryGenerationContext.Get(_query);
                    qgc.Bind(_query);
                    childTable.Alias = qgc.NextTableAlias();
                }
                //添加子表查询与父实体的关系条件：WHERE c.CategoryId = b.Id
                var parentProperty = ((IRepository)childRepo).GetParentProperty();
                var parentRefIdProperty = parentProperty.RefIdProperty;
                var toParentConstraint = f.Constraint(childTable.Column(parentRefIdProperty.Name), ownerTable.IdColumn);
                subQuery.Where = f.And(toParentConstraint, subQuery.Where);
                var last = _lastJoinTable;
                _lastJoinTable = childTable;
                subQuery.Where = f.And(subQuery.Where, (IConstraint)Process(theOperand.Condition));
                _lastJoinTable = last;
                return f.Exists(subQuery);
            }
            IQueryNode node = null;
            if (!object.ReferenceEquals(theOperand.AggregatedExpression, null))
                node = (IQueryNode)Process(theOperand.AggregatedExpression);
            else if (!object.ReferenceEquals(theOperand.Condition, null))
                node = (IQueryNode)Process(theOperand.Condition);
            if (theOperand.AggregateType == Aggregate.Max)
                return f.Function(node, SqlFunctions.MAX);
            if (theOperand.AggregateType == Aggregate.Min)
                return f.Function(node, SqlFunctions.MIN);
            if (theOperand.AggregateType == Aggregate.Count)
                return f.Function(node, SqlFunctions.COUNT);
            if (theOperand.AggregateType == Aggregate.Sum)
                return f.Function(node, SqlFunctions.SUM);
            if (theOperand.AggregateType == Aggregate.Avg)
                return f.Function(node, SqlFunctions.AVG);
            throw new NotSupportedException();
        }

        ITableSource _lastJoinTable;

        public object Visit(OperandProperty theOperand)
        {
            ITableSource ownerTable = _lastJoinTable ?? _query.MainTable;
            var propertyName = theOperand.PropertyName;
            if (!propertyName.Contains('.'))
            {
                //[s2:Category] s2为别名
                string alias = propertyName.Substring(0, Math.Max(0, propertyName.LastIndexOf(':')));
                var property = propertyName.Substring(propertyName.LastIndexOf(':') + 1);
                if (alias.IsNotEmpty())
                {
                    ITableSource aliasTable = null;
                    foreach (var q in queries)
                    {
                        aliasTable = q.From.FindTable(alias: alias);
                        if (aliasTable != null)
                            return aliasTable.Column(property);
                    }
                    throw new QueryException("找不到别名为[{0}]的表".FormatArgs(alias));
                }
                return ownerTable.Column(property);
            }
            //[s2:Category].[ItemMediumCategory].[ItemLargeCategory].[Name]
            string lastProperty = null;
            ITableSource last = null;
            foreach (var property in propertyName.Split('.'))
            {
                string alias = property.Substring(0, Math.Max(0, property.LastIndexOf(':')));
                lastProperty = property.Substring(property.LastIndexOf(':') + 1);
                if (last != null)
                    ownerTable = last;
                var p = ownerTable.EntityRepository.FindProperty(lastProperty);
                if (p is IRefEntityProperty)
                {
                    var refRepo = RF.Find(p.PropertyType);
                    var joinTable = _query.From.FindTable(refRepo, alias);
                    if (joinTable == null)
                    {
                        joinTable = f.FindOrCreateJoinTable(_query, ownerTable, (IRefEntityProperty)p);
                        if (alias.IsNotEmpty())
                            joinTable.Alias = alias;
                    }
                    last = joinTable;
                }
            }
            return last.Column(lastProperty);
        }

        public object Visit(JoinOperand theOperand)
        {
            throw new NotImplementedException();
        }
    }
}
