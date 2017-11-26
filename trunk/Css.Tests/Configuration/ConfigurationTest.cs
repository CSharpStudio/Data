using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Css.Configuration;
using Css.ComponentModel;
using System.IO;
using Css.IO;

namespace Css.Tests.Configuration
{
    public class ConfigurationTest : DisposableBase
    {
        [Fact]
        public void GetConnectionString()
        {
            JsonConfigSection section = new JsonConfigSection();
            JsonConfigSection connectionStringSection = new JsonConfigSection();
            connectionStringSection.Set("DbMes", new ConnectionStringSection
            {
                ConnectionString = "DataSource=",
                Name = "DbMes",
                ProviderName = "System.Data.SqlClient"
            });
            connectionStringSection.Set("DbMaster", new
            {
                ConnectionString = "DataSource=",
                Name = "DbMaster",
                ProviderName = "System.Data.SqlClient"
            });
            section.SetSection("ConnectionStrings", connectionStringSection);

            var result = section.Save();
            var config = new Config(FileName.Create("config.json"), section);
            var db = config.GetConnectionString("DbMaster");
            Assert.Equal("DbMaster", db.Name);
        }
    }
}
