using System.Linq;

namespace Css.Domain
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extenstions
    {
        /// <summary>
        /// 查找父引用属性
        /// </summary>
        /// <returns></returns>
        public static RefPropertyMeta FindParentProperty(this EntityMeta meta)
        {
            return meta.Properties.OfType<RefPropertyMeta>().FirstOrDefault(p => p.ReferenceType == ReferenceType.Parent);
        }
    }
}
