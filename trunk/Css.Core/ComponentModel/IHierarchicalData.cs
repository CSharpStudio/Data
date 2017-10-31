using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    /// <summary>
    /// 樹型結構
    /// </summary>
    public interface IHierarchicalData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets True if the node is expanded, False if it is collapsed.
        /// </summary>
        bool Expanded { get; set; }

        /// <summary>
        /// Gets a 1 base value that represents the depth of the current node in the node hierarchy.
        /// </summary>
        int Level { get; }

        bool HasChild { get; }
        /// <summary>
        /// Gets or sets the parent of this node.
        /// </summary>
        IHierarchicalData Parent { get; set; }

        /// <summary>
        /// Gets or set the nodes that are children to this node.
        /// </summary>
        BindingCollection<IHierarchicalData> Children { get; }
    }

    /// <summary>
    /// 樹型結構
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHierarchicalData<T> : INotifyPropertyChanged where T : IHierarchicalData<T>
    {
        /// <summary>
        /// Gets or sets True if the node is expanded, False if it is collapsed.
        /// </summary>
        bool Expanded { get; set; }

        /// <summary>
        /// Gets a 1 base value that represents the depth of the current node in the node hierarchy.
        /// </summary>
        int Level { get; }

        bool HasChild { get; }
        /// <summary>
        /// Gets or sets the parent of this node.
        /// </summary>
        T Parent { get; set; }

        /// <summary>
        /// Gets or set the nodes that are children to this node.
        /// </summary>
        BindingCollection<T> Children { get; }
    }
}

