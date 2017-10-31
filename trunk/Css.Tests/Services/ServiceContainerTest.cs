using Css.Services;
using System;
using Xunit;

namespace Css.Tests.Services
{
    public class ServiceContinarerTest
    {
        class SelfTypeService { }

        interface IService { }
        class ServiceImpl : IService { }

        public class MyRemoteService : RemoteService
        {
            //RemoteService's method must be virtual and the assembly mark internal visible to [DynamicProxyGenAssembly2]
            public string GetString()
            {
                return "The string";
            }
        }

        class InstanceService { }

        [Service(FallbackType = typeof(FallbackService))]
        class FallbackService { }

        [Fact]
        public void ResolveService()
        {
            var service = RT.Service.Resolve<IServiceContainer>();
            Assert.NotNull(service);
        }
        [Fact]
        public void RegisterTypeService()
        {
            RT.Service.Register<IService, ServiceImpl>();
            var service = RT.Service.Resolve<IService>();
            Assert.IsType<ServiceImpl>(service);
        }
        [Fact]
        public void RegisterInstanceService()
        {
            var instance = new InstanceService();
            RT.Service.Register<InstanceService>(instance);
            var service = RT.Service.Resolve<InstanceService>();
            Assert.Same(instance, service);
        }
        [Fact]
        public void ResolveFallbackService()
        {
            var service = RT.Service.Resolve<FallbackService>();
            Assert.IsType<FallbackService>(service);
        }
        [Fact]
        public void ResolveRemoteService()
        {
            var service = RT.Service.Resolve<MyRemoteService>();
            var result = service.GetString();
            Assert.IsNotType<MyRemoteService>(service);
        }
    }
}
