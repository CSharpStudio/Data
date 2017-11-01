using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Domain
{
    public interface IEntityRepository : IRepository
    {
        IList<IProperty> GetChildProperties();
        IRefEntityProperty ParentProperty { get; }
        IProperty FindProperty(string propertyName);
    }
}
