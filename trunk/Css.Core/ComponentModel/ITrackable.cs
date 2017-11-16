using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    /// <summary>
    /// 可追踪状态变更接口
    /// </summary>
    public interface ITrackable
    {
        /// <summary>
        /// 状态变更
        /// </summary>
        event EventHandler StateChanged;

        /// <summary>
        /// 状态
        /// </summary>
        DataState DataState { get; }

        /// <summary>
        /// 标记为创建状态<see cref="DataState.Created"/>.
        /// </summary>
        void MarkCreated();

        /// <summary>
        /// 标记为修改状态<see cref="DataState.Modified"/>.
        /// </summary>
        void MarkModified();

        /// <summary>
        /// 标记为删除状态<see cref="DataState.Deleted"/>.
        /// </summary>
        void MarkDeleted();

        /// <summary>
        /// 重置为普通状态<see cref="DataState.Normal"/>.
        /// </summary>
        void ResetState();

        /// <summary>
        /// 禁示变更通知
        /// </summary>
        void SuppressNotifyChanged();

        /// <summary>
        /// 恢复变更通知
        /// </summary>
        void ResumeNotifyChanged();

        /// <summary>
        /// 触发变更通知
        /// </summary>
        /// <param name="propertyName"></param>
        void RaisePropertyChanged(string propertyName);

        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        void Set<T>(T value, string propertyName);
    }
}
