using Common.Logging;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.StructureMap;
using Nancy.ViewEngines;
using StructureMap;

namespace OpenHomeServer.Server.Web
{
    public class NancyBootstrapper : StructureMapNancyBootstrapper
    {
        private static IContainer _container;

        public NancyBootstrapper()
        {
            ResourceViewLocationProvider.RootNamespaces.Add(GetType().Assembly, "OpenHomeServer.Server.Web.Views");
        }

        public static void SetApplicationContainer(IContainer container)
        {
            _container = container;
        }

        protected override IContainer GetApplicationContainer()
        {
            return _container;
        }        

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(x => {
                    x.ViewLocationProvider = typeof(ResourceViewLocationProvider);                    
                });
            }
        }

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.Add(
                Nancy.Embedded.Conventions.EmbeddedStaticContentConventionBuilder.AddDirectory("Scripts", this.GetType().Assembly, "Web\\Scripts")
            );                      
        }

        protected override void ApplicationStartup(IContainer kernel, IPipelines pipelines)
        {
            base.ApplicationStartup(kernel, pipelines);
            var logger = kernel.GetInstance<ILog>();

            pipelines.OnError.AddItemToStartOfPipeline((ctx, ex) =>
            {
                logger.Error("Something went wrong with Nancy", ex);
                return null;
            });
        }

#if DEBUG
        protected override Nancy.Diagnostics.DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                return new Nancy.Diagnostics.DiagnosticsConfiguration { Password="test" };
            }
        }
#endif
    }
}
