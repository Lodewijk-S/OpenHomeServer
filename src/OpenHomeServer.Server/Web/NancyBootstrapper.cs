using Common.Logging;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.ViewEngines;

namespace OpenHomeServer.Server.Web
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        public NancyBootstrapper()
        {
            ResourceViewLocationProvider.RootNamespaces.Add(GetType().Assembly, "OpenHomeServer.Server.Web.Views");
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

        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            var logger = LogManager.GetCurrentClassLogger(); //container.Resolve<Common.Logging.ILog>();

            pipelines.OnError += (ctx, ex) =>
            {
                logger.Error("", ex);
                return null;
            };
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
