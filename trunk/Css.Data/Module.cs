using Css.Data;
using Css.Data.Common;
using Css.Modules;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: Module(typeof(Module))]
namespace Css.Data
{
    class Module : AppModule
    {
        static Module()
        {
            DbManager.SetManager(new DbManagerImpl());
        }

        public override void Initialize(IApp app)
        {
        }
    }
}
