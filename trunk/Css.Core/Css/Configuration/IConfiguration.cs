using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Configuration
{
    public interface IConfiguration
    {
        void Load();
        void Save();

    }
}
