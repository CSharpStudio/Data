using Css.Data;
using Css.Domain.Query.Impl;
using System;

namespace Css.Domain.Query
{
    /// <summary>
    /// 如果需要遍历 IQuery 对象，可以使用本类来进行访问。
    /// 
    /// Css.Domain.Query 中的所有接口，构成了新的面向 Entity、IProperty 的查询语法树。
    /// </summary>
    public abstract class QueryNodeVisitor
    {
        protected virtual IQueryNode Visit(IQueryNode node)
        {
            switch (node.NodeType)
            {
                case QueryNodeType.Query:
                    return VisitQuery(node as IQuery);
                case QueryNodeType.SubQuery:
                    return VisitSubQueryRef(node as ISubQuery);
                case QueryNodeType.Array:
                    return VisitArray(node as IArray);
                case QueryNodeType.TableSource:
                    return VisitEntitySource(node as ITableSource);
                case QueryNodeType.SelectAll:
                    return VisitSelectAll(node as ISelectAll);
                case QueryNodeType.Join:
                    return VisitJoin(node as IJoin);
                case QueryNodeType.OrderBy:
                    return VisitOrderBy(node as IOrderBy);
                case QueryNodeType.GroupBy:
                    return VisitGroupBy(node as IGroupBy);
                case QueryNodeType.Column:
                    return VisitProperty(node as IColumnNode);
                case QueryNodeType.BinaryConstraint:
                    return VisitBinaryConstraint(node as IBinaryConstraint);
                case QueryNodeType.GroupConstraint:
                    return VisitGroupConstraint(node as IGroupConstraint);
                case QueryNodeType.ExistsConstraint:
                    return VisitExistsConstraint(node as IExistsConstraint);
                case QueryNodeType.NotConstraint:
                    return VisitNotConstraint(node as INotConstraint);
                case QueryNodeType.Literal:
                    return VisitLiteral(node as ILiteral);
                case QueryNodeType.Function:
                    return VisitFunction(node as IFunctionNode);
                case QueryNodeType.Value:
                    return VisitValue(node as IValueNode);
                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual IValueNode VisitValue(IValueNode node)
        {
            return node;
        }

        protected virtual IFunctionNode VisitFunction(IFunctionNode node)
        {
            return node;
        }

        protected virtual IJoin VisitJoin(IJoin node)
        {
            Visit(node.Left);
            Visit(node.Right);
            Visit(node.Condition);
            return node;
        }

        protected virtual IGroupConstraint VisitGroupConstraint(IGroupConstraint node)
        {
            Visit(node.Left);
            VisitGroupOperator(node.Opeartor);
            Visit(node.Right);
            return node;
        }

        protected virtual GroupOp VisitGroupOperator(GroupOp op)
        {
            return op;
        }

        protected virtual IQuery VisitQuery(IQuery node)
        {
            if (node.Selection != null)
            {
                Visit(node.Selection);
            }
            Visit(node.From);
            if (node.Where != null)
            {
                Visit(node.Where);
            }
            var entityQuery = node as TableQuery;
            if (entityQuery.HasGroup())
            {
                for (int i = 0, c = node.GroupBy.Count; i < c; i++)
                {
                    var item = node.GroupBy[i];
                    Visit(item);
                }
            }
            if (entityQuery.HasOrdered())
            {
                for (int i = 0, c = node.OrderBy.Count; i < c; i++)
                {
                    var item = node.OrderBy[i];
                    Visit(item);
                }
            }
            return node;
        }

        protected virtual ITableSource VisitEntitySource(ITableSource node)
        {
            return node;
        }

        protected virtual IColumnNode VisitProperty(IColumnNode node)
        {
            return node;
        }

        protected virtual IBinaryConstraint VisitBinaryConstraint(IBinaryConstraint node)
        {
            Visit(node.Left);
            VisitBinaryOperator(node.Operator);
            Visit(node.Right);
            return node;
        }

        protected virtual BinaryOp VisitBinaryOperator(BinaryOp op)
        {
            return op;
        }

        protected virtual ILiteral VisitLiteral(ILiteral node)
        {
            return node;
        }

        protected virtual IArray VisitArray(IArray node)
        {
            for (int i = 0, c = node.Items.Count; i < c; i++)
            {
                var item = node.Items[i];
                Visit(item);
            }
            return node;
        }

        protected virtual ISelectAll VisitSelectAll(ISelectAll node)
        {
            return node;
        }

        protected virtual IExistsConstraint VisitExistsConstraint(IExistsConstraint node)
        {
            Visit(node.Query);
            return node;
        }

        protected virtual INotConstraint VisitNotConstraint(INotConstraint node)
        {
            Visit(node.Constraint);
            return node;
        }

        protected virtual ISubQuery VisitSubQueryRef(ISubQuery node)
        {
            Visit(node.Query);
            return node;
        }

        protected virtual IQueryNode VisitOrderBy(IOrderBy node)
        {
            Visit(node.Column);
            return node;
        }

        protected virtual IQueryNode VisitGroupBy(IGroupBy node)
        {
            Visit(node.Column);
            return node;
        }
    }
}