using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Css.Services
{
    public class RemoteServiceInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.TargetType.IsDefined(typeof(LocalAttribute))
                || invocation.Method.IsDefined(typeof(LocalAttribute)))
            {
                invocation.Proceed();
                return;
            }
            //TODO:Invoke Through Service
            invocation.Proceed();
        }
    }
}
