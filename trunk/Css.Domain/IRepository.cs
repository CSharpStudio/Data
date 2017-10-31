using Css.Data;
using Css.Domain.Metadata;
using Css.Domain.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public interface IRepository
    {
        Type EntityType { get; }
        Type EntityListType { get; }
        EntityMeta EntityMeta { get; }

        IList<IProperty> GetChildProperties();
        IRefEntityProperty ParentProperty { get; }
        IProperty FindProperty(string propertyName);

        Entity GetById(object id);
        IEntityList GetByParentId(object id);
        void Save(IDomain entity);

        int Count(IQuery query);
        IEntityList ToList(IQuery query, int start, int end);
        Entity FirstOrDefault(IQuery query);
    }

    public interface IDbRepository
    {
        DbSetting DbSetting { get; }
    }
}
