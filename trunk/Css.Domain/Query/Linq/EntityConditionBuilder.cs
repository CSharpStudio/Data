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
    class EntityConditionBuilder : ExpressionVisitor
    {
        /// <summary>
        /// 正在组织的查询对象。
        /// </summary>
        private IConstraint _where;
        private ITableSource _mainTabel;
        private IRepository _repo;
        private QueryFactory f = QueryFactory.Instance;

        /// <summary>
        /// 是否需要反转查询中的所有条件操作符。
        /// 场景：当转换 Linq 表达式中的 All 方法到 Sql 的 NotExsits 时，需要把内部的条件都转换为反向操作符。
        /// </summary>
        private bool _reverseWhere = false;

        public EntityConditionBuilder(IRepository repo) : this(repo, false) { }

        internal EntityConditionBuilder(IRepository repo, bool reverseWhere)
        {
            _repo = repo;
            _reverseWhere = reverseWhere;
        }

        internal IConstraint Build(Expression exp)
        {
            _mainTabel = f.Table(_repo);
            Visit(exp);

            return _where;
        }

        #region 处理方法调用

        protected override Expression VisitMethodCall(MethodCallExpression exp)
        {
            var method = exp.Method;
            var methodType = method.DeclaringType;
            bool processed = false;

            //处理 Queryable 上的方法
            if (methodType == typeof(EntityUpdate) || methodType == typeof(EntityDelete))
            {
                processed = VisitMethod_Queryable(exp);
            }
            //处理 string 上的方法
            else if (methodType == typeof(string))
            {
                processed = VisitMethod_String(exp);
            }
            else if (methodType == typeof(Enumerable))
            {
                processed = VisitMethod_Enumerable(exp);
            }
            else if (methodType.IsGenericType && methodType.GetGenericTypeDefinition() == typeof(List<>))
            {
                processed = VisitMethod_List(exp);
            }

            if (!processed) throw OperationNotSupported(method);

            return exp;
        }

        private bool VisitMethod_Queryable(MethodCallExpression exp)
        {
            var args = exp.Arguments;
            if (args.Count == 2)
            {
                Visit(args[0]);//visit queryable

                var lambda = StripQuotes(args[1]) as LambdaExpression;

                var previousWhere = _where;

                Visit(lambda.Body);

                switch (exp.Method.Name)
                {
                    case LinqMethods.Where:
                        //如果现在不是第一次调用 Where 方法，那么需要把本次的约束和之前的约束进行 And 合并。
                        MakeBooleanConstraintIfNoValue();
                        if (_where != null && previousWhere != null)
                        {
                            _where = f.And(previousWhere, _where);
                        }
                        break;
                    default:
                        break;
                }

                return true;
            }

            return false;
        }

        private bool VisitMethod_String(MethodCallExpression exp)
        {
            var args = exp.Arguments;
            switch (exp.Method.Name)
            {
                case LinqMethods.Contains:
                    _operator = _hasNot ? BinaryOp.NotContains : BinaryOp.Contains;
                    Visit(exp.Object);
                    Visit(args[0]);
                    break;
                case LinqMethods.StartWith:
                    _operator = _hasNot ? BinaryOp.NotStartsWith : BinaryOp.StartsWith;
                    Visit(exp.Object);
                    Visit(args[0]);
                    break;
                case LinqMethods.EndWith:
                    _operator = _hasNot ? BinaryOp.NotEndsWith : BinaryOp.EndsWith;
                    Visit(exp.Object);
                    Visit(args[0]);
                    break;
                case LinqMethods.IsNullOrEmpty:
                    _valueResult = string.Empty;
                    _hasValueResult = true;
                    _operator = _hasNot ? BinaryOp.NotEqual : BinaryOp.Equal;
                    Visit(args[0]);
                    break;
                default:
                    throw OperationNotSupported(exp.Method);
            }

            MakeConstraint();

            return true;
        }

        private bool VisitMethod_Enumerable(MethodCallExpression exp)
        {
            var args = exp.Arguments;
            switch (exp.Method.Name)
            {
                case LinqMethods.Contains:
                    if (args.Count == 2)
                    {
                        _operator = _hasNot ? BinaryOp.NotIn : BinaryOp.In;
                        Visit(args[1]);//先访问属性
                        Visit(args[0]);//再访问列表常量
                        MakeConstraint();
                        return true;
                    }
                    break;
                //case LinqConsts.EnumerableMethod_Any:
                //case LinqConsts.EnumerableMethod_All:
                //    var subQueryBuilder = new SubEntityQueryBuilder();
                //    _constraintResult = subQueryBuilder.Build(exp, f.Query(_repo), PropertyFinder);
                //    MakeConstraint();
                //    return true;
                default:
                    break;
            }
            return false;
        }

        private bool VisitMethod_List(MethodCallExpression exp)
        {
            switch (exp.Method.Name)
            {
                case LinqMethods.Contains:
                    _operator = _hasNot ? BinaryOp.NotIn : BinaryOp.In;
                    Visit(exp.Arguments[0]);//先访问属性
                    Visit(exp.Object);//再访问列表常量
                    MakeConstraint();
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
                    Visit(node.Operand);
                    MakeBooleanConstraintIfNoValue();
                    _hasNot = false;
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    Visit(node.Operand);
                    break;
                default:
                    throw OperationNotSupported(node);
            }
            return node;
        }

        #endregion

        #region 属性访问

        private SimplePropertyFinder _propertyFinder;

        private SimplePropertyFinder PropertyFinder
        {
            get
            {
                if (_propertyFinder == null)
                {
                    _propertyFinder = new SimplePropertyFinder(_repo);
                }
                return _propertyFinder;
            }
        }

        private IConstraint _nullableRefConstraint;

        protected override Expression VisitMember(MemberExpression m)
        {
            var pf = PropertyFinder;
            var property = pf.Find(m);

            //访问值属性
            VisitValueProperty(property.Name, _mainTabel);

            return m;
        }

        #endregion

        #region 构造属性约束条件

        //当前的属性条件
        private IColumnNode _propertyResult;

        //操作符
        private BinaryOp? _operator;

        //对比的目标值或属性
        private bool _hasValueResult;//由于 _valueResult 可以表示 null，所以需要一个额外的字段来判断当前是否有值。
        private object _valueResult;
        private IColumnNode _rightPropertyResult;

        private IConstraint _constraintResult;

        protected override Expression VisitBinary(BinaryExpression binaryExp)
        {
            if (binaryExp.NodeType == ExpressionType.AndAlso || binaryExp.NodeType == ExpressionType.OrElse)
            {
                //先计算左边的约束结果
                Visit(binaryExp.Left);
                MakeBooleanConstraintIfNoValue();
                var left = _where;

                //再计算右边的约束结果
                Visit(binaryExp.Right);
                MakeBooleanConstraintIfNoValue();
                var right = _where;

                //使用 AndOrConstraint 合并约束的结果。
                var op = binaryExp.NodeType == ExpressionType.AndAlso ?
                    GroupOp.And : GroupOp.Or;
                if (_reverseWhere)
                {
                    op = binaryExp.NodeType == ExpressionType.AndAlso ? GroupOp.Or : GroupOp.And;
                }
                _where = f.Binary(left, op, right);
            }
            else
            {
                VisitPropertyComparison(binaryExp);
            }

            return binaryExp;
        }

        private void VisitPropertyComparison(BinaryExpression binaryExp)
        {
            //收集属性、值
            Visit(binaryExp.Left);
            Visit(binaryExp.Right);

            //转换为操作符
            MakeOperator(binaryExp);

            //生成属性条件
            MakeConstraint();
        }

        private void MakeOperator(BinaryExpression binaryExp)
        {
            //var method = _queryMethod.Peek();
            //if (method == QueryMethod.Queryable)
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

                _where = _constraintResult;
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
            if (_propertyResult != null && _propertyResult.PropertyType == typeof(bool) &&
                !_operator.HasValue &&
                _valueResult == null && _rightPropertyResult == null
                )
            {
                _operator = BinaryOp.Equal;
                _valueResult = _hasNot ? false : true;
                _hasValueResult = true;
                MakeConstraint();
            }
        }

        void VisitValueProperty(string propertyName, ITableSource mpOwnerTable)
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
            if (node.Value is IQueryable) return node;

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
