namespace Css.Domain
{
    /// <summary>
    /// 领域接口，实体<see cref="IEntity"/>和实体列表<see cref="IEntityList"/>都属于领域范畴。
    /// </summary>
    public interface IDomain
    {
        void SetRefParent(IEntity entity);

        /// <summary>
        /// 获取领域的仓库 <see cref="IRepository"/>. 每个实体类型有唯一单例的仓库。
        /// </summary>
        /// <returns></returns>
        IRepository GetRepository();
    }
}
