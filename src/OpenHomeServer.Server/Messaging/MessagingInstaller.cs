using OpenHomeServer.Server.Messaging.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Conventions.Syntax;

namespace OpenHomeServer.Server.Messaging
{
    public class MessagingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<SignalRNinjectDependencyResolver>().ToSelf();
            Bind(typeof(HubContextFactory<>)).ToSelf();
            
            Kernel.Bind(x => x.FromThisAssembly().SelectAllClasses().InheritedFrom<IHubPipelineModule>().BindSingleInterface());
            Kernel.Bind(x => x.FromThisAssembly().SelectAllClasses().InheritedFrom<IHub>().BindToSelf());
        }
    }

    public class HubContextFactory<T> where T : Hub
    {
        IConnectionManager _connectionManager;

        public HubContextFactory(SignalRNinjectDependencyResolver resolver)
        {
            _connectionManager = resolver.Resolve<IConnectionManager>();
        }

        public IHubContext GetContext()
        {
            return _connectionManager.GetHubContext<T>();
        }
    }
}
