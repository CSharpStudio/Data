using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Domain
{
    public interface IEntityRepository : IRepository
    {
        VarPropertyContainer PropertyContainer { get; }
    }
}
