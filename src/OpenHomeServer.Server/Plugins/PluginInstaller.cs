using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Facilities.Startable;
using System;

namespace OpenHomeServer.Server.Plugins
{
    public class PluginInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().BasedOn<IRunAtStartUp>().Configure(s => s.Start())
            );
        }
    }
}
