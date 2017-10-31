using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    /// <summary>
    /// State of data
    /// </summary>
    public enum DataState
    {
        /// <summary>
        /// Normal state.
        /// </summary>
        Normal,
        /// <summary>
        /// The object was created by the client and then need to be inserting into database.
        /// </summary>
        Created,
        /// <summary>
        /// The object was deleted by the client and then need to be deleting from database.
        /// </summary>
        Deleted,
        /// <summary>
        /// The object has been modified by the client and then need to be updating from database.
        /// </summary>
        Modified,
    }
}
