using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Facilities.Startable;

namespace OpenHomeServer.Server.Plugins
{
    public class PluginInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().BasedOn<IRunAtStartUp>().Configure(s => s.Start()),
                Classes.FromThisAssembly().Where(t => t.Name.EndsWith("Repository")),
                Classes.FromThisAssembly().Where(t => t.Name.EndsWith("Service"))
            );
        }
    }
}
