using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 实体，属性领域对象。每个实体有唯一键。
    /// </summary>
    public interface IEntity : IDomain, ICloneable, INotifyPropertyChanged, ITrackable
    {
        /// <summary>
        /// 获取当前实体唯一键。
        /// </summary>
        /// <returns></returns>
        object GetId();

        /// <summary>
        /// 设置当前实体唯一键。
        /// </summary>
        /// <param name="id">唯一键</param>
        void SetId(object id);

        /// <summary>
        /// 为实体生成唯一键。
        /// </summary>
        /// <returns></returns>
        void GenerateId();
    }
}
