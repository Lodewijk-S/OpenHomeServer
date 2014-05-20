using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeServer.Server.Storage
{
    public class StorageInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For(typeof(Persister<>)).DependsOn(Dependency.OnValue("applicationName", "OpenHomeServer"))
            );
        }
    }
}
