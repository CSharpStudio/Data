using Css.Data;
using Css.Data.Common;
using Css.Domain.Query.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Css.Domain.Query
{
    /// <summary>
    /// 查询构造工厂
    /// </summary>
    public class QueryFactory
    {
        public static readonly QueryFactory Instance = new QueryFactory();

        private QueryFactory() { }

        /// <summary>
        /// 为指定的仓库构造一个查询。
        /// </summary>
        /// <param name="mainTableRepository">主表对应的实体的仓库。</param>
        /// <returns></returns>
        public IQuery Query(IRepository mainTableRepository)
        {
            var source = this.Table(mainTableRepository);
            return this.Query(source);
        }

        /// <summary>
        /// 为指定的仓库构造一个查询。
        /// </summary>
        /// <typeparam name="TEntity">主表对应的实体。</typeparam>
        /// <returns></returns>
        public IQuery Query<TEntity>()
            where TEntity : Entity
        {
            var source = this.Table<TEntity>();
            return this.Query(source);
        }

        /// <summary>
        /// 构造一个查询对象。
        /// </summary>
        /// <param name="from">要查询的数据源。</param>
        /// <param name="selection">
        /// 要查询的内容。
        /// 如果本属性为空，表示要查询所有数据源的所有属性。
        /// </param>
        /// <param name="where">查询的过滤条件。</param>
        /// <param name="orderBy">
        /// 查询的排序规则。
        /// 可以指定多个排序条件。
        /// </param>
        /// <param name="isCounting">
        /// 是否只查询数据的条数。
        /// 
        /// 如果这个属性为真，那么不再需要使用 Selection。
        /// </param>
        /// <param name="isDistinct">是否需要查询不同的结果。</param>
        /// <returns></returns>
        public IQuery Query(
            ISource from,
            IQueryNode selection = null,
            IConstraint where = null,
            List<IOrderBy> orderBy = null,
            bool isCounting = false,
            bool isDistinct = false
            )
        {
            if (from == null) throw new ArgumentNullException("from");

            var tableQuery = new TableQuery();
            IQuery query = tableQuery;
            query.IsCounting = isCounting;
            query.IsDistinct = isDistinct;
            query.Selection = selection;
            query.From = from;
            query.Where = where;
            tableQuery.SetOrderBy(orderBy);
            if (from.NodeType == QueryNodeType.TableSource)
            {
                tableQuery.MainTable = from as ITableSource;
            }
            else
            {
                tableQuery.MainTable = from.FindTable();
            }
            return query;
        }

        /// <summary>
        /// 构造一个数组节点。
        /// </summary>
        /// <param name="nodes">所有数组中的项。</param>
        /// <returns></returns>
        public IArray Array(params IQueryNode[] nodes)
        {
            return Array(nodes as IEnumerable<IQueryNode>);
        }

        /// <summary>
        /// 构造一个数组节点。
        /// </summary>
        /// <param name="nodes">所有数组中的项。</param>
        /// <returns></returns>
        public IArray Array(IEnumerable<IQueryNode> nodes)
        {
            IArray array = new ArrayNode();

            array.Items = new List<IQueryNode>(nodes);

            return array;
        }

        /// <summary>
        /// 构造一个数组节点。
        /// </summary>
        /// <param name="nodes">所有数组中的项。</param>
        /// <returns></returns>
        internal IArray AutoSelectionColumns(IEnumerable<IQueryNode> nodes)
        {
            IArray array = new AutoSelectionColumns();

            array.Items = new List<IQueryNode>(nodes);

            return array;
        }

        /// <summary>
        /// 构造一个 SelectAll 节点。
        /// </summary>
        /// <param name="table">如果本属性为空，表示选择所有数据源的所有属性；否则表示选择指定数据源的所有属性。</param>
        /// <returns></returns>
        public ISelectAll SelectAll(ITableSource table = null)
        {
            ISelectAll res = new SelectAll();
            res.Source = table;
            return res;
        }

        /// <summary>
        /// 构造一个实体的数据源节点。
        /// </summary>
        /// <typeparam name="TEntity">本实体数据源来自于这个实体对应的仓库。</typeparam>
        /// <param name="alias">同一个实体仓库可以表示多个不同的数据源。这时，需要这些不同的数据源指定不同的别名。</param>
        /// <returns></returns>
        public ITableSource Table<TEntity>(string alias = null)
            where TEntity : Entity
        {
            return Table(RF.Find<TEntity>(), alias);
        }

        /// <summary>
        /// 构造一个实体的数据源节点。
        /// </summary>
        /// <param name="repository">本实体数据源来自于这个实体仓库。</param>
        /// <param name="alias">同一个实体仓库可以表示多个不同的数据源。这时，需要这些不同的数据源指定不同的别名。</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">entityRepository</exception>
        public ITableSource Table(IRepository repository, string alias = null)
        {
            if (repository == null) throw new ArgumentNullException("entityRepository");

            var tableInfo = (repository as EntityRepository).TableInfo;
            if (tableInfo == null)
            {
                throw new ORMException("类型 {0} 不能直接用于 ORM 查询。这是因为该实体类没有映射数据库。如果该类是一个实体基类，请在查询方法中指定 propertyOwner 参数为具体的子实体类型。".FormatArgs(repository.EntityType));
            }

            //构造一个 EntitySource 对象。
            //在构造 TableSource 时，不必立刻为所有属性生成相应的列。必须使用懒加载。
            var table = new TableSource();
            table._tableInfo = tableInfo;

            var res = table as ITableSource;
            res.EntityRepository = repository;
            table.TableName = tableInfo.Name;
            table.Alias = alias;

            return table;
        }

        /// <summary>
        /// 在查询对象中查找或者创建指定引用属性对应的连接表对象。
        /// </summary>
        /// <param name="query">需要在这个查询对象中查找或创建连接表。</param>
        /// <param name="propertyOwner">引用属性对应外键所在的表。</param>
        /// <param name="refProperty">指定的引用属性。</param>
        /// <returns></returns>
        public ITableSource FindOrCreateJoinTable(IQuery query, ITableSource propertyOwner, IRefEntityProperty refProperty)
        {
            return (query as TableQuery).FindOrCreateJoinTable(propertyOwner, refProperty);
        }

        //暂时去除
        ///// <summary>
        ///// 在查询对象中查找或者创建指定引用属性对应的连接表对象。
        ///// </summary>
        ///// <param name="query">需要在这个查询对象中查找或创建连接表。</param>
        ///// <param name="propertyOwner">聚合子属性所在的实体对应的表。也是外键关系中主键表所在的表。</param>
        ///// <param name="childrenProperty">指定的聚合子属性。</param>
        ///// <returns></returns>
        //public ITableSource FindOrCreateJoinTable(IQuery query, ITableSource propertyOwner, IListProperty childrenProperty)
        //{
        //    return (query as TableQuery).FindOrCreateJoinTable(propertyOwner, childrenProperty);
        //}

        /// <summary>
        /// 构造一个属性与指定值"相等"的约束条件节点。
        /// </summary>
        /// <param name="node">要对比的属性。</param>
        /// <param name="value">要对比的值。</param>
        /// <returns></returns>
        public IConstraint Constraint(
            IQueryNode node,
            object value
            )
        {
            return Constraint(node, BinaryOp.Equal, value);
        }

        /// <summary>
        /// 构造一个属性的约束条件节点。
        /// </summary>
        /// <param name="node">要对比的属性。</param>
        /// <param name="op">对比操作符</param>
        /// <param name="value">要对比的值。</param>
        /// <returns></returns>
        public IConstraint Constraint(
            IQueryNode node,
            BinaryOp op,
            object value
            )
        {
            if (op == BinaryOp.In || op == BinaryOp.NotIn)
            {
                List<IQueryNode> values = new List<IQueryNode>();
                foreach (var v in value as IEnumerable)
                    values.Add(Value(v));
                return Constraint(node, op, Array(values));
            }
            return Constraint(node, op, Value(value));
        }

        /// <summary>
        /// 构造一个两个属性"相等"的约束条件节点。
        /// </summary>
        /// <param name="leftNode">第一个需要对比的列。</param>
        /// <param name="rightNode">第二个需要对比的列。</param>
        /// <returns></returns>
        public IConstraint Constraint(
            IQueryNode leftNode,
            IQueryNode rightNode
            )
        {
            return Constraint(leftNode, BinaryOp.Equal, rightNode);
        }

        /// <summary>
        /// 构造一个两个属性进行对比的约束条件节点。
        /// </summary>
        /// <param name="leftNode">第一个需要对比的列。</param>
        /// <param name="op">对比条件。</param>
        /// <param name="rightNode">第二个需要对比的列。</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// leftProperty
        /// or
        /// rightProperty
        /// </exception>
        public IConstraint Constraint(
            IQueryNode leftNode,
            BinaryOp op,
            IQueryNode rightNode
            )
        {
            if (leftNode == null) throw new ArgumentNullException("leftProperty");
            if (rightNode == null) throw new ArgumentNullException("rightProperty");

            IBinaryConstraint res = new Impl.BinaryConstraint();
            res.Left = leftNode;
            res.Right = rightNode;
            res.Operator = op;
            return res;
        }

        /// <summary>
        /// 构造一个 是否存在查询结果的约束条件节点
        /// </summary>
        /// <param name="query">要检查的查询。</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">query</exception>
        public IConstraint Exists(IQuery query, bool isNot = false)
        {
            if (query == null) throw new ArgumentNullException("query");

            IExistsConstraint res = new Impl.ExistsConstraint();
            res.Query = query;
            res.IsNot = isNot;
            return res;
        }

        /// <summary>
        /// 构造一个对指定约束条件节点执行取反规则的约束条件节点。
        /// </summary>
        /// <param name="constraint">需要被取反的条件。</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">constraint</exception>
        public IConstraint Not(IConstraint constraint)
        {
            if (constraint == null) throw new ArgumentNullException("constraint");

            if (constraint is NotConstraint)
            {
                return (constraint as NotConstraint).Constraint as IConstraint;
            }

            INotConstraint res = new NotConstraint();
            res.Constraint = constraint;
            return res;
        }

        /// <summary>
        /// 构造一个查询文本。
        /// </summary>
        /// <param name="formattedSql">查询文本。</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public ILiteral Literal(string formattedSql, params object[] parameters)
        {
            if (string.IsNullOrEmpty(formattedSql)) throw new ArgumentNullException("text");

            ILiteral res = new Impl.Literal();
            res.FormattedSql = formattedSql;
            res.Parameters = parameters;
            return res;
        }

        /// <summary>
        /// 对指定的所有约束构造一个 And 连接串节点。
        /// </summary>
        /// <param name="constraints">需要使用 And 进行连接的所有约束。</param>
        /// <returns></returns>
        public IConstraint And(params IConstraint[] constraints)
        {
            if (constraints.Length < 2) throw new ArgumentException("最少必须指定两个条件。");

            var res = constraints[0];
            for (int i = 1, c = constraints.Length; i < c; i++)
            {
                var item = constraints[i];
                res = And(res, item);
            }
            return res;
        }

        /// <summary>
        /// 构造一个 And 连接的节点。
        /// </summary>
        /// <param name="left">二位运算的左操作结点。</param>
        /// <param name="right">二位运算的右操作节点。</param>
        public IConstraint And(IConstraint left, IConstraint right)
        {
            return Binary(left, GroupOp.And, right);
        }

        /// <summary>
        /// 对指定的所有约束构造一个 Or 连接串节点。
        /// </summary>
        /// <param name="constraints">需要使用 Or 进行连接的所有约束。</param>
        /// <returns></returns>
        public IConstraint Or(params IConstraint[] constraints)
        {
            if (constraints.Length < 2) throw new ArgumentException("最少必须指定两个条件。");

            var res = constraints[0];
            for (int i = 1, c = constraints.Length; i < c; i++)
            {
                var item = constraints[i];
                res = Or(res, item);
            }
            return res;
        }

        /// <summary>
        /// 构造一个 Or 连接的节点。
        /// </summary>
        /// <param name="left">二位运算的左操作结点。</param>
        /// <param name="right">二位运算的右操作节点。</param>
        public IConstraint Or(IConstraint left, IConstraint right)
        {
            return Binary(left, GroupOp.Or, right);
        }

        /// <summary>
        /// 构造一个二位操作符连接的节点。
        /// </summary>
        /// <param name="left">二位运算的左操作结点。</param>
        /// <param name="op">二位运算类型。</param>
        /// <param name="right">二位运算的右操作节点。</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// left
        /// or
        /// right
        /// </exception>
        public IConstraint Binary(
            IConstraint left,
            GroupOp op,
            IConstraint right
            )
        {
            //如果其中一个约束为空，则直接返回另一个约束。
            if (left == null) return right;
            if (right == null) return left;

            IGroupConstraint res = new Impl.GroupConstraint();
            res.Left = left;
            res.Opeartor = op;
            res.Right = right;
            return res;
        }

        /// <summary>
        /// 通过左实体数据源和右实体数据源，来找到它们之间的第一个的引用属性，用以构造一个连接。
        /// </summary>
        /// <param name="left">拥有引用关系的左数据源。</param>
        /// <param name="right">右实体数据源。</param>
        /// <returns></returns>
        public IJoin Join(
            ITableSource left,
            ITableSource right
            )
        {
            //通过实体的对比，找到从左实体到右实体之间的第一个的引用属性。
            var rightEntity = right.EntityRepository.EntityType;
            var properties = left.EntityRepository.EntityMeta.Properties;
            foreach (var property in properties)
            {
                var refProperty = property as RefPropertyMeta;
                if (refProperty != null && refProperty.PropertyType == rightEntity)
                {
                    var condition = this.Constraint(
                        left.Column(refProperty.PropertyName),
                        right.Column(Entity.IdProperty.Name)
                    );

                    var joinType = refProperty.Nullable ? JoinType.LeftOuter : JoinType.Inner;

                    return Join(left, right, condition, joinType);
                }
            }

            throw new InvalidProgramException(string.Format(
                "没有从 {0} 到 {1} 的引用关系，请指定具体的对比条件。", left.GetName(), right.GetName()
                ));
        }

        /// <summary>
        /// 通过左数据源和右实体数据源，以及从左到右的引用属性，来构造一个连接。
        /// </summary>
        /// <param name="left">左实体数据源。</param>
        /// <param name="right">右实体数据源。</param>
        /// <param name="leftToRight">从左到右的引用属性。</param>
        /// <returns></returns>
        public IJoin Join(
            ITableSource left,
            ITableSource right,
            IRefProperty leftToRight
            )
        {
            var condition = this.Constraint(
                left.Column(leftToRight.Name),
                right.Column(Entity.IdProperty.Name)
            );

            var joinType = leftToRight.Nullable ? JoinType.LeftOuter : JoinType.Inner;

            return Join(left, right, condition, joinType);
        }

        /// <summary>
        /// 通过左数据源和右实体数据源，以及从左到右的引用属性，来构造一个连接。
        /// </summary>
        /// <param name="left">左数据源。</param>
        /// <param name="right">右实体数据源。</param>
        /// <param name="leftToRight">从左到右的引用属性。</param>
        /// <returns></returns>
        public IJoin Join(
            ISource left,
            ITableSource right,
            IRefProperty leftToRight
            )
        {
            var leftSource = left.FindTable(RF.Find(leftToRight.OwnerType));

            var condition = this.Constraint(
                leftSource.Column(leftToRight.Name),
                right.Column(Entity.IdProperty.Name)
            );

            var joinType = leftToRight.Nullable ? JoinType.LeftOuter : JoinType.Inner;

            return Join(left, right, condition, joinType);
        }

        /// <summary>
        /// 构造一个数据源与实体数据源连接后的结果节点
        /// </summary>
        /// <param name="left">左边需要连接的数据源。</param>
        /// <param name="right">右边需要连接的数据源。</param>
        /// <param name="condition">连接所使用的约束条件。</param>
        /// <param name="joinType">连接方式</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// left
        /// or
        /// right
        /// or
        /// condition
        /// </exception>
        public IJoin Join(
            ISource left,
            ITableSource right,
            IConstraint condition,
            JoinType joinType = JoinType.Inner
            )
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));
            Check.NotNull(condition, nameof(condition));

            IJoin res = new Impl.Join();
            res.Left = left;
            res.JoinType = joinType;
            res.Right = right;
            res.Condition = condition;
            return res;
        }

        /// <summary>
        /// 构造一个子查询。
        /// </summary>
        /// <param name="query">内部的查询对象。</param>
        /// <param name="alias">必须对这个子查询指定别名。</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// query
        /// or
        /// alias
        /// </exception>
        public ISubQuery SubQuery(IQuery query, string alias)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNullOrEmpty(alias, nameof(alias));

            ISubQuery res = new Impl.SubQueryRef();
            res.Query = query;
            res.Alias = alias;
            return res;
        }

        /// <summary>
        /// 构造一个排序节点。
        /// </summary>
        /// <param name="property">使用这个属性进行排序。</param>
        /// <param name="direction">使用这个方向进行排序。</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">property</exception>
        public IOrderBy OrderBy(IColumnNode property, ListSortDirection direction = ListSortDirection.Ascending)
        {
            Check.NotNull(property, nameof(property));

            IOrderBy res = new Impl.OrderBy();
            res.Column = property;
            res.Direction = direction;
            return res;
        }

        public IGroupBy GroupBy(IColumnNode property)
        {
            Check.NotNull(property, nameof(property));

            IGroupBy res = new Impl.GroupBy();
            res.Column = property;
            return res;
        }

        /// <summary>
        /// <see cref="SqlFunctions"/> 定义的函数
        /// </summary>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <param name="alais"></param>
        /// <returns></returns>
        public IFunctionNode Function(List<IQueryNode> args, string name, string alais = null)
        {
            Check.NotNullOrEmpty(args, nameof(args));
            Check.NotNullOrEmpty(name, nameof(name));
            IFunctionNode fun = new FunctionNode();
            fun.Function = name;
            fun.Alias = alais;
            fun.Args.Items = args;
            return fun;
        }
        public IFunctionNode Function(IQueryNode arg, string name, string alais = null)
        {
            Check.NotNull(arg, nameof(arg));
            Check.NotNullOrEmpty(name, nameof(name));
            IFunctionNode fun = new FunctionNode();
            fun.Function = name;
            fun.Alias = alais;
            fun.Args.Items = new[] { arg };
            return fun;
        }

        /// <summary>
        /// <see cref="SqlBinaryFunctions"/> 定义的函数
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="name"></param>
        /// <param name="alais"></param>
        /// <returns></returns>
        public IFunctionNode BinaryFunction(IQueryNode left, IQueryNode right, string name, string alais = null)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));
            Check.NotNullOrEmpty(name, nameof(name));
            IFunctionNode fun = new BinaryFunctionNode();
            fun.Function = name;
            fun.Alias = alais;
            fun.Args.Items = new List<IQueryNode>() { left, right };
            return fun;
        }

        public IValueNode Value(object value)
        {
            return new ValueNode { Value = value };
        }

        //internal void Generate(SqlGenerator generator, IQuery query, int start = 0, int end = 0)
        //{
        //    generator.Generate(query as TableQuery, start, end);
        //}
    }
}