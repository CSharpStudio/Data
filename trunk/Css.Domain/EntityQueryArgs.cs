using Css.Domain.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 使用 IQuery 进行查询的参数。
    /// </summary>
    public class EntityQueryArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityQueryArgs"/> class.
        /// </summary>
        public EntityQueryArgs() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityQueryArgs"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public EntityQueryArgs(IQuery query)
        {
            Query = query;
        }

        /// <summary>
        /// 对应的查询条件定义。
        /// </summary>
        public IQuery Query { get; set; }
    }
}
