using Common.Logging;
using OpenHomeServer.Server.Jobs;
using StructureMap.Configuration.DSL;

namespace OpenHomeServer.Server.DuctTape
{
    public class MainRegistry : Registry
    {

        public MainRegistry()
        {
            //Logging   
            For<ILog>().Use(c => LogManager.GetLogger(c.RequestedName));

            ForConcreteType<OpenHomeServerService>();
            ForConcreteType<JobOrganiser>();
            ForConcreteType<OwinWebApplicationHost>();
        }
    }
}