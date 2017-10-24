using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace Css.Tests.Configuration
{
    public class ConnectionStringTest
    {
        [Fact]
        public void GetConnectionString()
        {
            var children = RT.Config.GetChildren();
            var con = RT.Config.GetSection("ConnectionStrings");
            var c = con.GetChildren();

            var cs = RT.Config.GetConnectionString("Name");
        }
    }
}
