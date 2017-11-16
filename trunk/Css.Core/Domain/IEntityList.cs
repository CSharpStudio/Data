using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 实体列表接口。表示实体<see cref="IEntity"/>集合，与实体一样也属于领域对象<see cref="IDomain"/>
    /// </summary>
    public interface IEntityList : IList, IDomain
    {
        /// <summary>
        /// 集合无数的实体<see cref="IEntity"/>类型
        /// </summary>
        Type EntityType { get; }

        void SyncParentEntityId(IEntity id);

        /// <summary>
        /// 删除的实体集合
        /// </summary>
        IList Deleted { get; }

        /// <summary>
        /// 实体总数量，表示未分页的总记录数，<see cref="ICollection.Count"/>表示当前集合的数量
        /// </summary>
        int Total { get; set; }
    }
}
