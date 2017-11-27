using Css.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Data.Common
{
    public interface IDbTable
    {
        ISqlDialect SqlDialect { get; }
        string Name { get; }
        IDbColumn PKColumn { get; }

        int Execute(IDbAccesser dba, ExecuteArgs args);
        ISqlGenerator CreateSqlGenerator();
        void Insert(IDbAccesser dba, object entity);
        int Update(IDbAccesser dba, object entity);
        int Delete(IDbAccesser dba, object entity);
    }
}
