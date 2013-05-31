using Microsoft.AspNet.SignalR;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer8.Server.Infrastructure
{
    public class TinyIoCDependencyResolver : DefaultDependencyResolver
    {
        private readonly TinyIoCContainer _container;

        public TinyIoCDependencyResolver(TinyIoCContainer container)
        {
            _container = container;            
        }

        public override object GetService(Type serviceType)
        {
            return _container.CanResolve(serviceType) ? _container.Resolve(serviceType) : base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var objects = _container.CanResolve(serviceType) ? _container.ResolveAll(serviceType) : new object[] { };
            return objects.Concat(base.GetServices(serviceType));
        }
    }
}
