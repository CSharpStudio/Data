using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ConnectionForRepositoryAttribute : Attribute
    {
        public ConnectionForRepositoryAttribute(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        public string ConnectionStringName { get; set; }
    }
}
