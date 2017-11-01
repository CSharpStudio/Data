using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Css.Domain.Query.Linq
{
    /// <summary>
    /// 访问一个属性的表达式，如 A.B.C.Name，并执行以下操作：
    /// * 对其中使用到的引用属性，在查询对象中添加表的 Join。
    /// * 返回找到的最终属性（如上面的 Name，或者是 A.Children 中的 Children），以及该属性对应的表对象。
    /// * 如果引用属性是可空引用属性，则同时还会生成该可空引用属性不为空的条件。（因为是对这个引用实体的属性进行判断，所以需要这个引用不能为空。）
    /// </summary>
    class EntityPropertyFinder : ExpressionVisitor
    {
        private IQuery _query;
        private IRepository _repo;
        private bool _reverseConstraint;

        private QueryFactory f = QueryFactory.Instance;

        internal EntityPropertyFinder(IQuery query, IRepository repo, bool reverseConstraint)
        {
            _query = query;
            _repo = repo;
            _reverseConstraint = reverseConstraint;
        }

        /// <summary>
        /// 是否需要在查询中反转所有条件。
        /// </summary>
        public bool ReverseConstraint
        {
            get { return _reverseConstraint; }
        }

        /// <summary>
        /// 查找到的属性。
        /// </summary>
        public IProperty Property;

        /// <summary>
        /// 查找到的属性对应的表。
        /// </summary>
        public ITableSource PropertyOwnerTable;

        /// <summary>
        /// 如果使用了引用属性，而且是可空的引用属性，那么添加这可空外键不为空的条件。
        /// 这个属性将返回这个条件，外界使用时，需要主动将这个条件添加到查询中。
        /// 
        /// 例如：
        /// Book.Category.Name = 'a'
        /// 应该转换为
        /// Book.CategoryId IS NOT NULL AND BookCategory.Name = 'a'；
        /// 如果同时 <see cref="_reverseConstraint"/> 是 true，则应该转换为
        /// Book.CategoryId IS NULL OR BookCategory.Name != 'a'；
        /// </summary>
        public IConstraint NullableRefConstraint;
        Dictionary<string, ITableSource> _Tables;
        public void Find(Expression m, Dictionary<string, ITableSource> tables)
        {
            this.NullableRefConstraint = null;
            if (tables == null)
                throw new ArgumentNullException(nameof(tables));
            _Tables = tables;
            this.Visit(m);
        }

        /// <summary>
        /// 关联操作的最后一个引用属性。
        /// 用于在访问 A.B.C.Name 时记录 C；在访问完成后，值回归到 null。
        /// </summary>
        private IRefEntityProperty _lastJoinRefResult;
        private ITableSource _lastJoinTable;

        /// <summary>
        /// 是否当前正在访问引用对象中的属性。
        /// 主要用于错误提示，引用属性不能进行对比。
        /// </summary>
        private bool _visitRefProperties;

        protected override Expression VisitMember(MemberExpression m)
        {
            //只能访问属性
            var clrProperty = m.Member as PropertyInfo;
            if (clrProperty == null) throw EntityQueryerBuilder.OperationNotSupported(m.Member);
            var ownerExp = m.Expression;
            if (ownerExp == null) throw EntityQueryerBuilder.OperationNotSupported(m.Member);

            //exp 如果是: A 或者 A.B.C，都可以作为属性查询。
            var nodeType = ownerExp.NodeType;
            if (nodeType != ExpressionType.Parameter && nodeType != ExpressionType.MemberAccess) throw EntityQueryerBuilder.OperationNotSupported(m.Member);

            //如果是 A.B.C.Name，则先读取 A.B.C，记录最后一个引用实体类型 C；剩下 .Name 给本行后面的代码读取。
            VisitRefEntity(ownerExp);

            //属性的拥有类型对应的仓库。
            //获取当前正在查询的实体对应的仓库对象。如果是级联引用表达式，则使用最后一个实体即可。
            var ownerTable = _query.MainTable;
            IRepository ownerRepo = _repo;
            ownerRepo = RF.Find(ownerExp.Type);
            var tables = _Tables.Values.Where(p => p.EntityRepository.EntityType == ownerExp.Type);
            if (tables.Count() == 1)
                ownerTable = tables.First();
            else if (ownerExp is ParameterExpression && _Tables.TryGetValue((ownerExp as ParameterExpression).Name.ToUpper(), out ownerTable))
            {
                if (ownerTable.EntityRepository.EntityType != ownerExp.Type)
                    throw new ArgumentOutOfRangeException("参数[{0},{1}]对应的实体类型不正确,请确保参数名与别名一致".FormatArgs((ownerExp as ParameterExpression).Name, ownerExp.Type.GetQualifiedName()));
            }

            if (_lastJoinRefResult != null)
            {
                //如果已经有引用属性在列表中，说明上层使用了 A.B.C.Name 这样的语法。
                //这时，Name 应该是 C 这个实体的值属性。
                ownerRepo = RF.Find(_lastJoinRefResult.PropertyType);
                ownerTable = _lastJoinTable;
                _lastJoinRefResult = null;
                _lastJoinTable = null;
            }

            if (ownerTable == null)
                throw new ORMException("参数[{0}.{1},{2}]对应的实体类型不正确,请确保参数名与别名一致".FormatArgs((ownerExp as ParameterExpression)?.Name, clrProperty.Name, ownerExp.Type.GetQualifiedName()));

            //查询托管属性
            var mp = EntityQueryerBuilder.FindProperty(ownerRepo, clrProperty);
            if (mp == null) throw EntityQueryerBuilder.OperationNotSupported("Linq 查询的属性必须是一个托管属性。");
            if (mp is IRefEntityProperty)
            {
                //如果是引用属性，说明需要使用关联查询。
                var refProperty = mp as IRefEntityProperty;
                ITableSource refTable = null;
                var refTables = _Tables.Values.Where(p => p.EntityRepository.EntityType == refProperty.PropertyType);
                if (refTables.Count() == 0)
                {
                    refTable = f.FindOrCreateJoinTable(_query, ownerTable, refProperty);
                    _Tables.Add(refTable.Alias.ToUpper(), refTable);
                }
                else if (refTables.Count() == 1)
                    refTable = refTables.First();
                else
                    throw new ORMException("实体[{0}]有多次关联，不能用引用属性[{1}]条件，无法识别属性对应的实体".FormatArgs(refProperty.PropertyType.Name, refProperty.Name));
                if (refProperty.Nullable)
                {
                    var column = ownerTable.Column(refProperty.RefIdProperty.Name);
                    NullableRefConstraint = _reverseConstraint ?
                        f.Or(NullableRefConstraint, column.Equal(null as object)) :
                        f.And(NullableRefConstraint, column.NotEqual(null as object));
                }

                //存储到字段中，最后的值属性会使用这个引用属性对应的引用实体类型来查找对应仓库。
                _lastJoinRefResult = refProperty;
                _lastJoinTable = refTable;
                return m;
            }

            if (_visitRefProperties)
            {
                throw EntityQueryerBuilder.OperationNotSupported(string.Format("不支持使用属性：{0}。这是因为它的拥有者是一个值属性，值属性只支持直接对比。", mp.Name));
            }

            //访问值属性
            PropertyOwnerTable = ownerTable;
            Property = mp;

            return m;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == LinqMethods.Get)
            {
                var c = node.Arguments[0] as ConstantExpression;
                var mp = c?.Value as IProperty;
                if (mp != null)
                {
                    ITableSource ownerTable = null;
                    var tables = _Tables.Values.Where(p => p.EntityRepository.EntityType == node.Object.Type);
                    if (tables.Count() == 1)
                        ownerTable = tables.First();
                    else if (node.Object is ParameterExpression && _Tables.TryGetValue((node.Object as ParameterExpression).Name.ToUpper(), out ownerTable))
                    {
                        if (ownerTable.EntityRepository.EntityType != node.Object.Type)
                            throw new ArgumentOutOfRangeException("参数[{0},{1}]对应的实体类型不正确,请确保参数名与别名一致".FormatArgs((node.Object as ParameterExpression).Name, node.Object.Type.GetQualifiedName()));
                    }
                    if (ownerTable == null)
                        throw new ORMException("参数[{0}.{1},{2}]对应的实体类型不正确,请确保参数名与别名一致".FormatArgs((node.Object as ParameterExpression)?.Name, mp.Name, node.Object.Type.GetQualifiedName()));
                    PropertyOwnerTable = ownerTable;
                    Property = mp;
                }
            }
            return node;
        }

        /// <summary>
        /// 如果是 A.B.C.Name，则先读取 A.B.C
        /// </summary>
        /// <param name="exp"></param>
        private void VisitRefEntity(Expression exp)
        {
            if (exp.NodeType == ExpressionType.MemberAccess)
            {
                var oldValue = _visitRefProperties;
                _visitRefProperties = true;

                Visit(exp);

                _visitRefProperties = oldValue;
            }
        }
    }
}
