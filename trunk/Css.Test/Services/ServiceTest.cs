using System;
using Css.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Css.Test.Services
{
    [TestClass]
    public class ServiceTest
    {
        [Service(FallbackType = typeof(MyRemoteService))]
        public class MyRemoteService : RemoteService
        {
            public virtual string GetData()
            {
                return "data";
            }
        }

        [Service(FallbackType = typeof(FallbackService))]
        class FallbackService
        {
            public string GetData()
            {
                return "data";
            }

        }

        interface IService { }

        class ServiceImpl : IService { }

        [TestMethod]
        public void RegisterService()
        {
            RT.Service.Register<IService, ServiceImpl>();
            var service = RT.Service.Resolve<IService>();
        }

        [TestMethod]
        public void ResolveService()
        {
            var service = RT.Service.Resolve<FallbackService>();
            var data = service.GetData();
        }

        [TestMethod]
        public void RemoteService()
        {
            var service = RT.Service.Resolve<MyRemoteService>();
            var data = service.GetData();
        }
    }
}
