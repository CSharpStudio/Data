namespace Css.Domain.Query
{
    /// <summary>
    /// 一个拥有名字、可被引用的数据源。
    /// </summary>
    public interface INamedSource : ISource
    {
        /// <summary>
        /// 获取需要引用本数据源时可用的名字。
        /// </summary>
        /// <returns></returns>
        string GetName();
    }
}
