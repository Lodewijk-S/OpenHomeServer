using Common.Logging;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.ViewEngines;
using Ninject;

namespace OpenHomeServer.Server.Web
{
    public class NancyBootstrapper : NinjectNancyBootstrapper
    {
        private static IKernel _kernel;

        public NancyBootstrapper()
        {
            ResourceViewLocationProvider.RootNamespaces.Add(GetType().Assembly, "OpenHomeServer.Server.Web.Views");
        }        

        public static void SetApplicationContainer(IKernel kernel)
        {
            _kernel = kernel;
        }

        protected override IKernel GetApplicationContainer()
        {
            return _kernel;
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

        protected override void ApplicationStartup(IKernel kernel, IPipelines pipelines)
        {
            base.ApplicationStartup(kernel, pipelines);
            var logger = kernel.Get<ILog>();

            pipelines.BeforeRequest.AddItemToStartOfPipeline((ctx) => {
                logger.Info("BeforeRequest: " + ctx.Request.Url.ToString());
                return null;
            });

            pipelines.AfterRequest.AddItemToStartOfPipeline((ctx) =>
            {
                logger.Info("AfterRequest: " + ctx.Request.Url.ToString());
            });        

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
