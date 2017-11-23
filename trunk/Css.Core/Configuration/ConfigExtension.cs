using Css.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Css.Configuration
{
    public static class ConfigExtension
    {
        public static ConnectionStringSection GetConnectionString(this IConfig cfg, string name)
        {
            Check.NotNullOrEmpty(name, nameof(name));
            return cfg.GetList<ConnectionStringSection>("ConnectionStrings").FirstOrDefault(p => name.CIEquals(p.Name));
        }

        public static ConfigManager UserJsonConfig(this ConfigManager config, string fileName)
        {
            Check.NotNullOrEmpty(fileName, nameof(fileName));
            var file = DirectoryName.Create(AppDomain.CurrentDomain.BaseDirectory).CombineFile(fileName);
            RT.Config = new Config(file, JsonConfigSection.Load(file));
            return config;
        }

        public static ConfigManager UserXmlConfig(this ConfigManager config, string fileName)
        {
            Check.NotNullOrEmpty(fileName, nameof(fileName));
            var file = DirectoryName.Create(AppDomain.CurrentDomain.BaseDirectory).CombineFile(fileName);
            RT.Config = new Config(file, XmlConfigSection.Load(file));
            return config;
        }

        public static ConfigManager UserConfig(this ConfigManager config, IConfig cfg)
        {
            Check.NotNull(cfg, nameof(cfg));
            RT.Config = cfg;
            return config;
        }
    }
}
