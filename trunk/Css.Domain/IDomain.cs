using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public interface IDomain
    {
        void SetRefParent(Entity entity);

        /// <summary>
        /// Returns <see cref="IRepository"/> for the entity. Each type of entity has a singleton repository.
        /// </summary>
        /// <returns></returns>
        IRepository GetRepository();
    }
}
