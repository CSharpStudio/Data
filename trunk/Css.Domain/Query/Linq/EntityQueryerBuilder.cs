using Css.Data;
using Css.Domain.Metadata;
using Css.Domain.Query.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query.Linq
{
    /// <summary>
    /// 通过 Linq 表达式，调用 SqlSelect 来构造查询。
    /// </summary>
    class EntityQueryerBuilder : ExpressionVisitor
    {
        /// <summary>
        /// 正在组织的查询对象。
        /// </summary>
        private IQuery _query;
        private IRepository _repo;
        private QueryFactory f = QueryFactory.Instance;
        Dictionary<string, ITableSource> _allTables = new Dictionary<string, ITableSource>();
        IConstraint _constraint;
        /// <summary>
        /// 是否需要反转查询中的所有条件操作符。
        /// 场景：当转换 Linq 表达式中的 All 方法到 Sql 的 NotExsits 时，需要把内部的条件都转换为反向操作符。
        /// </summary>
        private bool _reverseWhere = false;

        public NewExpression NewExpression { get; set; }

        public EntityQueryerBuilder(IRepository repo) : this(repo, false) { }

        internal EntityQueryerBuilder(IRepository repo, bool reverseWhere)
        {
            _repo = repo;
            _reverseWhere = reverseWhere;
        }

        internal IQuery BuildQuery(Expression exp, string alias = null)
        {
            var mainTable = f.Table(_repo);
            _query = f.Query(mainTable);
            mainTable.Alias = alias ?? QueryGenerationContext.Get(_query).NextTableAlias();
            _allTables.Add(mainTable.Alias.ToUpper(), mainTable);
            Visit(exp);
            return _query;
        }

        internal void BuildQuery(Expression exp, IQuery query)
        {
            _query = query;
            Visit(exp);
        }

        #region 处理方法调用

        protected override Expression VisitMethodCall(MethodCallExpression exp)
        {
            var method = exp.Method;
            var methodType = method.DeclaringType;
            bool processed = false;

            //处理 EntityQueryer 上的方法
            if (methodType == typeof(EntityQueryer) ||
                methodType.IsGenericType && methodType.GetGenericTypeDefinition() == typeof(IEntityQueryer<>))
            {
                processed = VisitQueryerMethod(exp);
            }
            else if (methodType == typeof(FunctionExtension))
            {
                if (method.Name == LiteralVisitor.MethodName)
                    VisitLiteral(exp);
                else
                    VisitFunction(exp);
                processed = true;
            }
            else if (method.Name == LinqMethods.Get)
            {
                var pf = PropertyFinder;
                pf.Find(exp, _allTables);
                var column = _propertyFinder.PropertyOwnerTable.Column(_propertyFinder.Property.Name);
                _nullableRefConstraint = pf.NullableRefConstraint;

                //访问值属性
                VisitValueProperty(pf.Property.Name, pf.PropertyOwnerTable);
                processed = true;
            }
            //处理 string 上的方法
            else if (methodType == typeof(string))
            {
                processed = VisitStringMethod(exp);
            }
            else if (methodType == typeof(Enumerable))
            {
                processed = VisitEnumerableMethod(exp);
            }
            else if (methodType.IsGenericType && methodType.GetGenericTypeDefinition() == typeof(List<>))
            {
                processed = VisitListMethod(exp);
            }

            if (!processed) throw OperationNotSupported(method);

            return exp;
        }

        void VisitFunction(Expression expr)
        {
            var funVisitor = new FunctionVisitor(_allTables, PropertyFinder);
            funVisitor.Visit(expr);
            if (_propertyResult != null)//如果已经记录了条件的属性，那么当前的 mp 就是用于对比的第二个属性。（A.Code = A.Name 中的 Name）
                _rightPropertyResult = funVisitor.FunctionNode;
            else                //如果还没有记录属性，说明当前条件要比较的属性就是 mp；(A.Code = 1 中的 Code）
                _propertyResult = funVisitor.FunctionNode;
        }

        void VisitLiteral(Expression expr)
        {
            var visitor = new LiteralVisitor();
            visitor.Visit(expr);
            if (_propertyResult != null)//如果已经记录了条件的属性，那么当前的 mp 就是用于对比的第二个属性。（A.Code = A.Name 中的 Name）
                _rightPropertyResult = visitor.Literal;
            else                //如果还没有记录属性，说明当前条件要比较的属性就是 mp；(A.Code = 1 中的 Code）
                _propertyResult = visitor.Literal;
        }

        void VisitSelect(Expression expr)
        {
            var selection = new SelectionVisitor(_allTables, PropertyFinder);
            selection.Visit(expr);
            if (_query.Selection != null)
                throw new MethodAccessException("Selection不为空,[{0}]方法只能使用一次".FormatArgs(LinqMethods.Select));
            _query.Selection = f.AutoSelectionColumns(selection.Columns);
            NewExpression = selection.NewExpression;
        }

        void VisitJoin(LambdaExpression lambda, string alias, JoinType joinType)
        {
            var type = lambda.Parameters[1].Type;
            var right = RF.Find(type);
            var table = f.Table(right, alias ?? QueryGenerationContext.Get(_query).NextTableAlias());
            _allTables.Add(table.Alias.ToUpper(), table);
            this.Visit(lambda.Body);
            _query.From = f.Join(_query.From, table, _constraint, joinType);
        }

        void VisitOrderBy(Expression epxr, OrderDirection dir)
        {
            var orderBy = new SelectionVisitor(_allTables, PropertyFinder);
            orderBy.Visit(epxr);
            foreach (var column in orderBy.Columns.OfType<IColumnNode>())
                (_query as TableQuery).OrderBy.Add(f.OrderBy(column, dir));
        }

        bool VisitQueryerMethod(string methodName, LambdaExpression lambda, string alias)
        {
            switch (methodName)
            {
                case LinqMethods.Select:
                    {
                        VisitSelect(lambda.Body);
                        break;
                    }
                case LinqMethods.Join:
                    {
                        VisitJoin(lambda, alias, JoinType.Inner);
                        break;
                    }
                case LinqMethods.LeftJoin:
                    {
                        VisitJoin(lambda, alias, JoinType.LeftOuter);
                        break;
                    }
                case LinqMethods.RightJoin:
                    {
                        VisitJoin(lambda, alias, JoinType.RightOuter);
                        break;
                    }
                case LinqMethods.Where:
                    this.Visit(lambda.Body);
                    //如果现在不是第一次调用 Where 方法，那么需要把本次的约束和之前的约束进行 And 合并。
                    this.MakeBooleanConstraintIfNoValue();
                    _query.Where = f.And(_query.Where, _constraint);
                    break;
                case LinqMethods.Having:
                    this.Visit(lambda.Body);
                    //如果现在不是第一次调用 Where 方法，那么需要把本次的约束和之前的约束进行 And 合并。
                    this.MakeBooleanConstraintIfNoValue();
                    _query.Having = f.And(_query.Having, _constraint);
                    break;
                case LinqMethods.OrderBy:
                    VisitOrderBy(lambda.Body, OrderDirection.Ascending);
                    break;
                case LinqMethods.OrderByDescending:
                    VisitOrderBy(lambda.Body, OrderDirection.Descending);
                    break;
                case LinqMethods.GroupBy:
                    var groupBy = new SelectionVisitor(_allTables, PropertyFinder);
                    groupBy.Visit(lambda.Body);
                    foreach (var column in groupBy.Columns.OfType<IColumnNode>())
                        (_query as TableQuery).GroupBy.Add(f.GroupBy(column));
                    break;
                case LinqMethods.Exists:
                    Exists(lambda, false);
                    break;
                case LinqMethods.NotExists:
                    Exists(lambda, true);
                    break;
                default:
                    return false;
            }
            return true;
        }

        void Exists(LambdaExpression lambda, bool isNot)
        {
            var repo = RF.Find(lambda.Parameters.Last().Type.GetGenericArguments()[0]);//最后一个参数就是子查询的MainTable实体类型
            var mainTable = f.Table(repo);
            var query = f.Query(mainTable, f.Literal("1"));
            QueryGenerationContext.Get(_query).Bind(query);
            mainTable.Alias = QueryGenerationContext.Get(query).NextTableAlias();
            _allTables.Add(mainTable.Alias.ToUpper(), mainTable);
            var builder = new EntityQueryerBuilder(repo);
            builder._allTables = _allTables;
            builder.BuildQuery(lambda.Body, query);
            var exist = f.Exists(query, isNot);
            _query.Where = f.And(_query.Where, exist);
        }

        private bool VisitQueryerMethod(MethodCallExpression exp)
        {
            if (exp.Object != null)//如果是子查询,Object是IEntityQuery<T>,Argument少一个参数;如果是EntityQuery,Object是null,Arugment第一个参数是IEntityQuery<T>
                Visit(exp.Object);
            var args = exp.Arguments;
            if (exp.Method.Name == LinqMethods.Distinct)//Distinct 有一个或者没参数
            {
                foreach (var arg in args)
                    Visit(arg);
                _query.IsDistinct = true;
                return true;
            }
            string alias = null;
            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                if (i == args.Count - 1)
                {
                    var lambda = StripQuotes(arg) as LambdaExpression;
                    return VisitQueryerMethod(exp.Method.Name, lambda, alias);
                }
                if (arg is ConstantExpression)
                {
                    var constant = arg as ConstantExpression;
                    if (constant.Value is string)//Join多一个别名的参数
                    {
                        alias = constant.Value.ToString();
                        continue;
                    }
                }
                Visit(arg);
            }
            return false;
        }

        private bool VisitStringMethod(MethodCallExpression exp)
        {
            var args = exp.Arguments;
            switch (exp.Method.Name)
            {
                case LinqMethods.Contains:
                    _operator = _hasNot ? BinaryOp.NotContains : BinaryOp.Contains;
                    this.Visit(exp.Object);
                    this.Visit(args[0]);
                    break;
                case LinqMethods.StartWith:
                    _operator = _hasNot ? BinaryOp.NotStartsWith : BinaryOp.StartsWith;
                    this.Visit(exp.Object);
                    this.Visit(args[0]);
                    break;
                case LinqMethods.EndWith:
                    _operator = _hasNot ? BinaryOp.NotEndsWith : BinaryOp.EndsWith;
                    this.Visit(exp.Object);
                    this.Visit(args[0]);
                    break;
                case LinqMethods.IsNullOrEmpty:
                    _valueResult = string.Empty;
                    _hasValueResult = true;
                    _operator = _hasNot ? BinaryOp.NotEqual : BinaryOp.Equal;
                    this.Visit(args[0]);
                    break;
                default:
                    throw OperationNotSupported(exp.Method);
            }

            this.MakeConstraint();

            return true;
        }

        private bool VisitEnumerableMethod(MethodCallExpression exp)
        {
            var args = exp.Arguments;
            switch (exp.Method.Name)
            {
                case LinqMethods.Contains:
                    if (args.Count == 2)
                    {
                        _operator = _hasNot ? BinaryOp.NotIn : BinaryOp.In;
                        this.Visit(args[1]);//先访问属性
                        this.Visit(args[0]);//再访问列表常量
                        this.MakeConstraint();
                        return true;
                    }
                    break;
                case LinqMethods.Any:
                case LinqMethods.All:
                    var subQueryBuilder = new SubEntityQueryerBuilder();
                    _constraintResult = subQueryBuilder.Build(exp, _query, _allTables, PropertyFinder);
                    this.MakeConstraint();
                    return true;
                default:
                    break;
            }
            return false;
        }

        private bool VisitListMethod(MethodCallExpression exp)
        {
            switch (exp.Method.Name)
            {
                case LinqMethods.Contains:
                    _operator = _hasNot ? BinaryOp.NotIn : BinaryOp.In;
                    this.Visit(exp.Arguments[0]);//先访问属性
                    this.Visit(exp.Object);//再访问列表常量
                    this.MakeConstraint();
                    return true;
                default:
                    break;
            }

            return false;
        }

        #endregion

        #region 处理 Not

        /// <summary>
        /// 是否有 Not 操作。
        /// </summary>
        private bool _hasNot;

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    _hasNot = true;
                    this.Visit(node.Operand);
                    this.MakeBooleanConstraintIfNoValue();
                    _hasNot = false;
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    this.Visit(node.Operand);
                    break;
                default:
                    throw OperationNotSupported(node);
            }
            return node;
        }

        #endregion

        #region 属性访问

        private EntityPropertyFinder _propertyFinder;

        private EntityPropertyFinder PropertyFinder
        {
            get
            {
                if (_propertyFinder == null)
                {
                    _propertyFinder = new EntityPropertyFinder(_query, _repo, _reverseWhere);
                }
                return _propertyFinder;
            }
        }

        private IConstraint _nullableRefConstraint;

        protected override Expression VisitMember(MemberExpression m)
        {
            var pf = PropertyFinder;
            pf.Find(m, _allTables);
            _nullableRefConstraint = pf.NullableRefConstraint;

            //访问值属性
            VisitValueProperty(pf.Property.Name, pf.PropertyOwnerTable);

            return m;
        }

        #endregion

        #region 构造属性约束条件

        //当前的属性条件
        private IQueryNode _propertyResult;

        //操作符
        private BinaryOp? _operator;

        //对比的目标值或属性
        private bool _hasValueResult;//由于 _valueResult 可以表示 null，所以需要一个额外的字段来判断当前是否有值。
        private object _valueResult;
        private IQueryNode _rightPropertyResult;

        private IConstraint _constraintResult;

        protected override Expression VisitBinary(BinaryExpression binaryExp)
        {
            switch (binaryExp.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    //先计算左边的约束结果
                    this.Visit(binaryExp.Left);
                    this.MakeBooleanConstraintIfNoValue();
                    var left = _constraint;

                    //再计算右边的约束结果
                    this.Visit(binaryExp.Right);
                    this.MakeBooleanConstraintIfNoValue();
                    var right = _constraint;

                    //使用 AndOrConstraint 合并约束的结果。
                    var op = binaryExp.NodeType == ExpressionType.AndAlso ?
                        GroupOp.And : GroupOp.Or;
                    if (_reverseWhere)
                    {
                        op = binaryExp.NodeType == ExpressionType.AndAlso ? GroupOp.Or : GroupOp.And;
                    }
                    _constraint = f.Binary(left, op, right);
                    break;
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                    VisitFunction(binaryExp);
                    break;
                default:
                    VisitPropertyComparison(binaryExp);
                    break;
            }

            return binaryExp;
        }

        private void VisitPropertyComparison(BinaryExpression binaryExp)
        {
            //收集属性、值
            this.Visit(binaryExp.Left);
            this.Visit(binaryExp.Right);

            //转换为操作符
            this.MakeOperator(binaryExp);

            //生成属性条件
            this.MakeConstraint();
        }

        private void MakeOperator(BinaryExpression binaryExp)
        {
            if (_hasNot) throw OperationNotSupported("不支持操作符：'!'，请使用相反的操作符。");

            switch (binaryExp.NodeType)
            {
                case ExpressionType.Equal:
                    _operator = BinaryOp.Equal;
                    break;
                case ExpressionType.NotEqual:
                    _operator = BinaryOp.NotEqual;
                    break;
                case ExpressionType.LessThan:
                    _operator = BinaryOp.Less;
                    break;
                case ExpressionType.LessThanOrEqual:
                    _operator = BinaryOp.LessEqual;
                    break;
                case ExpressionType.GreaterThan:
                    _operator = BinaryOp.Greater;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _operator = BinaryOp.GreaterEqual;
                    break;
                default:
                    //throw new InvalidProgramException("两个属性间的比较只支持以下操作：=、!=、>、>=、<、<=。");
                    throw OperationNotSupported(binaryExp);
            }
        }

        /// <summary>
        /// 通过目前已经收集到的属性、操作符、值，来生成一个属性条件结果。
        /// 并清空已经收集的信息。
        /// </summary>
        private bool MakeConstraint()
        {
            if (_propertyResult != null && _operator.HasValue)
            {
                var op = _operator.Value;
                if (_reverseWhere) op = BinaryOperatorHelper.Reverse(op);

                if (_hasValueResult)
                {
                    _constraintResult = f.Constraint(_propertyResult, op, _valueResult);
                    _valueResult = null;
                    _hasValueResult = false;
                }
                else
                {
                    _constraintResult = f.Constraint(_propertyResult, op, _rightPropertyResult);
                    _rightPropertyResult = null;
                }
                _propertyResult = null;
                _operator = null;
            }

            if (_constraintResult != null)
            {
                if (_nullableRefConstraint != null)
                {
                    var concat = _reverseWhere ? GroupOp.Or : GroupOp.And;
                    _constraintResult = f.Binary(_nullableRefConstraint, concat, _constraintResult);
                    _nullableRefConstraint = null;
                }

                _constraint = _constraintResult;
                _constraintResult = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 如果只读取到了一个 Boolean 属性，没有读取到操作符、对比值，
        /// 而这时已经完成了条件的组装，那么必须把这个属性变成一个对判断条件。
        /// </summary>
        private void MakeBooleanConstraintIfNoValue()
        {
            if (_propertyResult != null &&
                (_propertyResult as IColumnNode)?.PropertyType == typeof(bool) &&
                !_operator.HasValue &&
                _valueResult == null &&
                _rightPropertyResult == null
                )
            {
                _operator = BinaryOp.Equal;
                _valueResult = _hasNot ? false : true;
                _hasValueResult = true;
                this.MakeConstraint();
            }

            if (_propertyResult != null &&
                _propertyResult is ILiteral &&
                !_operator.HasValue &&
                _valueResult == null &&
                _rightPropertyResult == null)
            {
                _constraintResult = (ILiteral)_propertyResult;
                _propertyResult = null;
                this.MakeConstraint();
            }
        }

        private void VisitValueProperty(string propertyName, ITableSource mpOwnerTable)
        {
            //如果已经记录了条件的属性，那么当前的 mp 就是用于对比的第二个属性。（A.Code = A.Name 中的 Name）
            if (_propertyResult != null)
            {
                _rightPropertyResult = mpOwnerTable.Column(propertyName);
            }
            //如果还没有记录属性，说明当前条件要比较的属性就是 mp；(A.Code = 1 中的 Code）
            else
            {
                _propertyResult = mpOwnerTable.Column(propertyName);
            }
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            //访问到根，直接返回。见 EntityQueryable 构造函数。
            if (node.Value is IEntityQueryer) return node;

            _valueResult = node.Value;
            _hasValueResult = true;

            return node;
        }

        #endregion

        #region 帮助方法

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = (e as UnaryExpression).Operand;
            }
            return e;
        }

        internal static IProperty FindProperty(IRepository info, PropertyInfo clrProperty)
        {
            return info.FindProperty(clrProperty.Name);
        }

        internal static Exception OperationNotSupported(MemberInfo member)
        {
            return new NotSupportedException(string.Format("不支持这个成员调用：'{1}'.'{0}'。", member.Name, member.DeclaringType.Name));
        }

        internal static Exception OperationNotSupported(Expression node)
        {
            return new NotSupportedException(string.Format("不支持类型为 {1} 的表达式：'{0}'。", node, node.NodeType));
        }

        internal static Exception OperationNotSupported(string msg)
        {
            return new NotSupportedException(msg);
            //return new NotSupportedException(string.Format("不支持这个操作：'{0}'。", action));
        }

        #endregion
    }
}
