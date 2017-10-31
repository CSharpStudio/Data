using Css.Domain.Query.Impl;
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
    class SelectionVisitor : ExpressionVisitor
    {
        public List<IQueryNode> Columns { get; set; } = new List<IQueryNode>();

        Dictionary<string, ITableSource> _tables;
        EntityPropertyFinder _propertyFinder;
        string _alias;

        public NewExpression NewExpression { get; set; }

        public SelectionVisitor(Dictionary<string, ITableSource> tables, EntityPropertyFinder propertyFinder)
        {
            _tables = tables;
            _propertyFinder = propertyFinder;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                _alias = node.Members[i].Name;
                Visit(node.Arguments[i]);
            }
            NewExpression = node;
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            ITableSource table = null;

            var tables = _tables.Values.Where(p => p.EntityRepository.EntityType == node.Type);
            if (tables.Count() == 1)
                table = tables.First();
            else if (_tables.TryGetValue(node.Name.ToUpper(), out table))
            {
                if (table.EntityRepository.EntityType != node.Type)
                    throw new ArgumentOutOfRangeException("参数[{0},{1}]对应的实体类型不正确,请确保参数名与别名一致".FormatArgs(node.Name, node.Type.GetQualifiedName()));
            }
            if (table == null)
                throw new ArgumentOutOfRangeException("找不到参数[{0},{1}]对应的实体,请确保实体已关联".FormatArgs(node.Name, node.Type.GetQualifiedName()));
            (table as TableSource).LoadAllColumns().ForEach(p =>
            {
                if (!p.ColumnName.CIEquals(p.PropertyName))
                    p.Alias = p.PropertyName;//转成属性名用于射成对象
                Columns.Add(p);
            });
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _propertyFinder.Find(node, _tables);
            var column = _propertyFinder.PropertyOwnerTable.Column(_propertyFinder.Property.Name);
            if (_alias.IsNotEmpty() && column.ColumnName != _alias)
                column.Alias = _alias;
            Columns.Add(column);
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var visitor = new FunctionVisitor(_tables, _propertyFinder);
            visitor.Visit(node);
            var function = visitor.FunctionNode;
            function.Alias = _alias;
            Columns.Add(function);
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == LinqMethods.Get)
            {
                _propertyFinder.Find(node, _tables);
                var column = _propertyFinder.PropertyOwnerTable.Column(_propertyFinder.Property.Name);
                if (_alias.IsNotEmpty() && column.ColumnName != _alias)
                    column.Alias = _alias;
                Columns.Add(column);
            }
            else if (node.Method.Name == LiteralVisitor.MethodName)
            {
                var visitor = new LiteralVisitor();
                visitor.Visit(node);
                Columns.Add(visitor.Literal);
            }
            else
            {
                var visitor = new FunctionVisitor(_tables, _propertyFinder);
                visitor.Visit(node);
                var function = visitor.FunctionNode;
                function.Alias = _alias;
                Columns.Add(function);
            }
            return node;
        }
    }
}
