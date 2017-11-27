using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Data.Common
{
    public interface ISqlGenerator
    {
        FormattedSql Sql { get; }

        void Generate(SqlNode tableQuery);
        void Generate(SqlSelect tableQuery, int start, int end);
    }
}
