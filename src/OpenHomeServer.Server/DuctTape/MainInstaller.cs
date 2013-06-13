using Common.Logging;
using Ninject;
using Ninject.Modules;
using OpenHomeServer.Server.Jobs;

namespace OpenHomeServer.Server.DuctTape
{
    public class MainModule : NinjectModule
    {

        public override void Load()
        {
            //Logging            
            Kernel.Bind<ILog>().ToMethod(c => LogManager.GetLogger(c.Binding.Service));

            Bind<OpenHomeServerService>().ToSelf();
            Bind<JobOrganiser>().ToSelf();
            Bind<OwinWebApplicationHost>().ToSelf();
        }
    }
}