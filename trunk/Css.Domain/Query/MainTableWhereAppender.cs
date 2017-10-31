using System.Collections.Generic;

namespace Css.Domain.Query
{
    /// <summary>
    /// 可以为查询中的主表的 Where 条件添加一个指定条件的类。
    /// </summary>
    public abstract class MainTableWhereAppender : QueryNodeVisitor
    {
        //修改_mainTableHandled为_handledMainTable,因为如果存在子查询，将会有多个MainTable
        //private bool _mainTableHandled;
        List<ITableSource> _handledMainTable = new List<ITableSource>();

        /// <summary>
        /// 是把新的条件添加到 Where 条件的最后。
        /// true：添加到最后。
        /// false：作为第一个条件插入。
        /// 默认：false。
        /// </summary>
        public bool AddConditionToLast { get; set; }

        /// <summary>
        /// 把条件添加到查询中的主表对应的 Where 中。
        /// </summary>
        /// <param name="node"></param>
        public void Append(IQuery node)
        {
            this.Visit(node);
        }

        /// <summary>
        /// 为所有的 IQuery 对象都添加相应的多租户查询。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override IQuery VisitQuery(IQuery node)
        {
            var query = base.VisitQuery(node);

            //if (!_mainTableHandled)
            //{
            if (!_handledMainTable.Contains(query.MainTable))
            {
                _handledMainTable.Add(query.MainTable);
                var mainTable = query.MainTable;
                var condition = this.GetCondition(mainTable, node);
                if (condition != null)
                {
                    if (this.AddConditionToLast)
                    {
                        query.Where = QueryFactory.Instance.And(query.Where, condition);
                    }
                    else
                    {
                        query.Where = QueryFactory.Instance.And(condition, query.Where);
                    }
                }
            }
            //    _mainTableHandled = true;
            //}

            return query;
        }

        /// <summary>
        /// 获取指定的主表对应的条件。
        /// </summary>
        /// <param name="mainTable">The main table.</param>
        /// <param name="query">The node.</param>
        /// <returns></returns>
        protected abstract IConstraint GetCondition(ITableSource mainTable, IQuery query);
    }
}
