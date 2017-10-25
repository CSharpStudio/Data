using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Test.Data
{
    [TestClass]
    public class ConnectionTest
    {
        [TestMethod]
        public void GetConnectionString()
        {
            var str = RT.Config.GetConnectionString("Master");
        }
    }
}
