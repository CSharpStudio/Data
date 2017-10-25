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
        IConfig _config;
        public ConfigurationTest()
        {
            var file = DirectoryName.Create(AppDomain.CurrentDomain.BaseDirectory).CombineFile("test.config");
            if (File.Exists(file))
                File.Delete(file);
            _config = new ConfigBuilder().LoadXmlFile("test.config").Build();
        }

        protected override void Cleanup()
        {
            base.Cleanup();
        }

        [Fact]
        public void GetSetValue()
        {
            _config.Set("Name", "MyName");
            Assert.Equal("MyName", _config.Get<string>("Name"));

            _config.Set("Int", 2);
            Assert.Equal(2, _config.Get<int>("Int"));

            _config.Set("Double", 2.3);
            Assert.Equal(2.3, _config.Get<double>("Double"));

            var now = DateTime.Now;
            _config.Set("Date", now);
            Assert.Equal(now.ToString("yyyyMMddHHmmss"), _config.Get<DateTime>("Date").ToString("yyyyMMddHHmmss"));

            _config.Save();
        }

        [Fact]
        public void GetSetListValue()
        {
            _config.SetList("NameList", new[] { "Name1", "Name2" });
            Assert.Equal(2, _config.GetList<string>("NameList").Count);

            _config.SetList("IntList", new[] { 1, 3, 4 });
            Assert.Equal(3, _config.GetList<int>("IntList").Count);

            _config.SetList("DoubleList", new[] { 1.2, 3.2, 4.2 });
            Assert.Equal(3, _config.GetList<double>("DoubleList").Count);

            var now = DateTime.Now;
            _config.SetList("DateList", new[] { now, now.AddHours(2) });
            Assert.Equal(2, _config.GetList<DateTime>("DateList").Count);

            _config.Save();
        }

        [Fact]
        public void GetDefaultValue()
        {
            var str = _config.Get("DefaultString", "DefaultString");
            var @int = _config.Get("DefaultInt", 2);
            var @double = _config.Get("DefaultDouble", 2.3);
            var date = _config.Get("DefaultDate", DateTime.Now);
            _config.Save();
        }

        [Fact]
        public void SaveLoad()
        {
            var now = DateTime.Now;
            _config.Set("String", "StringValue");
            _config.Set("Int", 2);
            _config.Set("Double", 2.3);
            _config.Set("Date", now);
            _config.Save();

            _config = new ConfigBuilder().LoadXmlFile("test.config").Build();

            Assert.Equal("StringValue", _config.Get<string>("String"));
            Assert.Equal(2, _config.Get<int>("Int"));
            Assert.Equal(2.3, _config.Get<double>("Double"));
            Assert.Equal(now.ToString("yyyyMMddHHmmss"), _config.Get<DateTime>("Date").ToString("yyyyMMddHHmmss"));
        }

        [Fact]
        public void GetSetSection()
        {
            _config.SetSection("Child", new XmlConfigSection());
            var section = _config.GetSection("Child");
            section.Set("Name", "ChildName");
            _config.Save();
        }

        [Fact]
        public void GetConnectionString()
        {
            _config.SetList<ConnectionStringSection>("ConnectionStrings", new[] {
                new ConnectionStringSection {
                    ConnectionString ="DataSource=",
                    Name ="DbMes",
                    ProviderName ="System.Data.SqlClient"
                },
                new ConnectionStringSection {
                    ConnectionString ="DataSource=",
                    Name ="DbMaster",
                    ProviderName ="System.Data.SqlClient"
                }
            });
            _config.Save();
            var db = _config.GetConnectionString("DbMaster");
            Assert.Equal("DbMaster", db.Name);
        }
    }
}
