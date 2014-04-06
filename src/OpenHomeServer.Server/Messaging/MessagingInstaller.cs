using Castle.MicroKernel.Registration;
using Castle.Facilities.TypedFactory;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Hubs;
using OpenHomeServer.Server.Plugins.Notifications;
using System.Reflection;
using Castle.MicroKernel;
using System;
using System.Collections;

namespace OpenHomeServer.Server.Messaging
{
    public class MessagingInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For<WindsorDependencyResolver>(),
                Classes.FromThisAssembly().BasedOn<IHubPipelineModule>().WithServiceFromInterface(),
                Classes.FromThisAssembly().BasedOn<IHub>(),
                Component.For<ITypedFactoryComponentSelector>().ImplementedBy<HubContextSelector>(),
                Component.For<IHubContextFactory>().AsFactory(c => c.SelectedWith<HubContextSelector>())
            );
        }
    }

    public interface IHubContextFactory
    {
        IHubContext CreateHubContext<THub>() where THub : IHub;
    }

    public class HubContextSelector : DefaultTypedFactoryComponentSelector
    {
        protected override IDictionary GetArguments(MethodInfo method, object[] arguments)
        {
            var dictionary = base.GetArguments(method, arguments);

            var hubType = method.GetGenericArguments()[0];
            dictionary.Add("hubname", hubType.Name);
            return dictionary;
        }

        protected override Func<IKernelInternal, IReleasePolicy, object> BuildFactoryComponent(MethodInfo method,string componentName,Type componentType,IDictionary additionalArguments)
        {
            return new HubContextResolver(componentName,componentType,additionalArguments,FallbackToResolveByTypeIfNameNotFound,GetType())
                .Resolve;
        }
    }

    public class HubContextResolver : TypedFactoryComponentResolver
    {
        public HubContextResolver(string componentName, Type componentType, IDictionary additionalArguments, bool fallbackToResolveByTypeIfNameNotFound, Type actualSelectorType)
            : base(componentName, componentType, additionalArguments, fallbackToResolveByTypeIfNameNotFound, actualSelectorType)
        {
        }

        public override object Resolve(IKernelInternal kernel, IReleasePolicy scope)
        {
            var manager = kernel.Resolve<WindsorDependencyResolver>().Resolve<IConnectionManager>();
            return manager.GetHubContext(additionalArguments["hubname"].ToString());
        }
    }
}
