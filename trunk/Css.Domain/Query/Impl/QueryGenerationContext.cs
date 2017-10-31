using Css.Domain.Query.Impl;

namespace Css.Domain.Query
{
    /// <summary>
    /// 生成 Sql 时的上下文对象。用于生成过程的上下文共享。
    /// </summary>
    internal class QueryGenerationContext
    {
        private int _tablesCount = 0;

        /// <summary>
        /// 当前已经使用过 <see cref="NextTableAlias"/> 生成的表的个数。
        /// </summary>
        public int TablesCount
        {
            get { return _tablesCount; }
        }

        /// <summary>
        /// 自动生成的 SQL 需要使用这个方法来统一生成表名。
        /// </summary>
        /// <returns></returns>
        public string NextTableAlias()
        {
            return "T" + _tablesCount++;
        }

        public static QueryGenerationContext Get(IQuery query)
        {
            var tq = query as TableQuery;
            if (tq.GenerationContext == null)
            {
                tq.GenerationContext = new QueryGenerationContext();
            }
            return tq.GenerationContext;
        }

        public void Bind(IQuery query)
        {
            (query as TableQuery).GenerationContext = this;
        }
    }
}