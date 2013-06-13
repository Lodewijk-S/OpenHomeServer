using Nancy;
using Nancy.Bootstrappers.Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using OpenHomeServer.Server.Web.Providers;

namespace OpenHomeServer.Server.Web
{
    public class WebModule : NinjectModule
    {
        public override void Load()
        {
            //A bit hackish, but this is how Nancy likes it
            NancyBootstrapper.SetApplicationContainer(Kernel);

            Kernel.Load(new[] { new FactoryModule() });
        }
    }
}
