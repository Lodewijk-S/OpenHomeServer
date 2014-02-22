using System;
using System.Collections.Specialized;
using Castle.Windsor;
using Common.Logging;
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
                using (var container = new WindsorContainer())
                {
                    container.Install(
                        new DuctTape.MainInstaller(), 
                        new Messaging.MessagingInstaller(), 
                        new Web.WebInstaller(),
                        new Plugins.PluginInstaller()
                        );

                    var service = container.Resolve<OpenHomeServerService>();

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
                Console.ReadKey();
            }
            catch (Exception e)
            {
                logger.Error("Cannot start the application", e);
            }
        }        
    }
}