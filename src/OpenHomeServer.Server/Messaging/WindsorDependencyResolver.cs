using Castle.Windsor;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenHomeServer.Server.Messaging
{
    public class WindsorDependencyResolver : DefaultDependencyResolver
    {
        private readonly IWindsorContainer _container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            _container = container;            
        }

        public override object GetService(Type serviceType)
        {
            return _container.Kernel.HasComponent(serviceType) ? _container.Resolve(serviceType) : base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var objects = _container.Kernel.HasComponent(serviceType) ? _container.ResolveAll(serviceType).Cast<Object>() : new object[] { };
            return objects.Concat(base.GetServices(serviceType));
        }
    }
}
