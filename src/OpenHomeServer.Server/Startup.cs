using Common.Logging;
using StructureMap;
using System;
using System.Collections.Specialized;
using Topshelf;

namespace OpenHomeServer.Server
{
    static class Startup
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
                using (var container = new Container(x => {
                    x.AddRegistry<DuctTape.MainRegistry>();
                    x.AddRegistry<Messaging.MessagingRegistry>();
                    x.AddRegistry<Jobs.JobRegistry>();
                    x.AddRegistry<Web.WebRegistry>();
                }))
                {
                    //A bit hackish, but this is how Nancy likes it
                    Web.NancyBootstrapper.SetApplicationContainer(container);

                    var service = container.GetInstance<OpenHomeServerService>();

                    //Start the service
                    HostFactory.Run(x =>
                        {
                            x.SetServiceName("OpenHomeServer");
                            x.SetDescription("Home Server Service for Windows");
                            x.SetDisplayName("OpenHomeServer Service");

                            x.StartAutomatically();

                            x.Service(() => service);
                        });
                }
            }
            catch (Exception e)
            {
                logger.Error("Cannot start the application", e);
            }
        }        
    }
}