namespace Css.Data.SqlTree
{
    /// <summary>
    /// 可查询的数据源，可用于 From 语句之后 。
    /// 目前有：SqlTable、SqlJoin、SqlSubSelect。
    /// </summary>
    public abstract class SqlSource : SqlNode, ISqlSource
    {
    }
}
