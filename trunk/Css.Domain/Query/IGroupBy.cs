using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    public interface IGroupBy : IQueryNode
    {
        /// <summary>
        /// 使用这个属性进行排序。
        /// </summary>
        IColumnNode Column { get; set; }
    }
}
