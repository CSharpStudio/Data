using Css.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Configuration
{
    public class ConfigBuilder
    {
        IConfigSection _section;
        FileName _file;

        public ConfigBuilder SetSection(IConfigSection section)
        {
            _section = section;
            return this;
        }

        public ConfigBuilder SetFile(FileName file)
        {
            _file = file;
            return this;
        }

        public IConfig Build()
        {
            var cfg = new Config(_file ?? new FileName("/appSettings.config"));
            cfg.Section = _section ?? new XmlConfigSection();
            return cfg;
        }
    }
}
