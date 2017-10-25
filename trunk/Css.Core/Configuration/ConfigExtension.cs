using Css.IO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Css.Configuration
{
    public static class ConfigExtension
    {
        public static ConfigBuilder LoadXmlFile(this ConfigBuilder builder, string fileName)
        {
            var file = DirectoryName.Create(AppDomain.CurrentDomain.BaseDirectory).CombineFile(fileName);
            return builder.SetFile(file).SetSection(XmlConfigSection.Load(file));
        }

        public static ConnectionStringSection GetConnectionString(this IConfig cfg, string name)
        {
            Check.NotNullOrEmpty(name, nameof(name));
            return cfg.GetList<ConnectionStringSection>("ConnectionStrings").FirstOrDefault(p => name.CIEquals(p.Name));
        }
    }
}
