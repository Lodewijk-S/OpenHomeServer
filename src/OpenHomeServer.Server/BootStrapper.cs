using Castle.Core;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Common.Logging;
using System;
using System.Collections.Specialized;
using Topshelf;

namespace OpenHomeServer.Server.Bootstrappers
{
    static class BootStrapper
    {
        public static void Main(string[] args)
        {
            //Setup Logging
            var properties = new NameValueCollection();
            LogManager.Adapter = new Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter(properties);
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                //Start the container
                using (var container = new WindsorContainer())
                {
                    container.Install(
                        new MainInstaller(), 
                        new Messaging.MessagingInstaller(), 
                        new Web.WebInstaller(),
                        new Jobs.JobInstaller()
                    );

                    //Start the service
                    HostFactory.Run(x =>
                    {
                        x.SetServiceName("OpenHomeServer");
                        x.SetDescription("Home Server Service for Windows");
                        x.SetDisplayName("OpenHomeServer Service");

                        x.StartAutomatically();

                        x.Service<ServiceBootstrapper>(() => container.Resolve<ServiceBootstrapper>());
                    });
                }
            }
            catch (Exception e)
            {
                logger.Error("Cannot start the application", e);
            }
        }        
    }

    public class MainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            //Container configuration
            container.AddFacility<TypedFactoryFacility>();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
            container.Register(Component.For<IWindsorContainer>().Instance(container));

            //Logging            
            container.Kernel.Resolver.AddSubResolver(new LoggerSubDependencyResolver());

            //Application parts
            container.Register(
                Component.For<ServiceBootstrapper>().ImplementedBy<ServiceBootstrapper>(),
                Component.For<ISchedulerHost>().ImplementedBy<QuartzScheduler>(),
                Component.For<IWebApplicationHost>().ImplementedBy<OwinWebApplicationHost>()
            );
        }
    }

    public class LoggerSubDependencyResolver : ISubDependencyResolver
    {
        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType == typeof(ILog);
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            if (CanResolve(context, contextHandlerResolver, model, dependency))
            {
                if (dependency.TargetType == typeof(ILog))
                {
                    return LogManager.GetLogger(model.Implementation);
                }
            }
            return null;
        }
    }
}
