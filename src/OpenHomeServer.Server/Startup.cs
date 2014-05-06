using System;
using System.Collections.Specialized;
using Castle.Windsor;
using Serilog;
using Serilog.Extras.Topshelf;
using Topshelf;

namespace OpenHomeServer.Server
{
    static class Startup
    {
        public static void Main(string[] args)
        {
            //Setup Logging
            var logging = new LoggerConfiguration()       
#if DEBUG
                .MinimumLevel.Debug()
#endif
                .WriteTo.ColoredConsole()
                .CreateLogger();
            Log.Logger = logging;

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

                            x.UseSerilog();

                            x.StartAutomatically();
                            
                            x.Service(() => service);                            
                        });
                }
                Console.ReadKey();
            }
            catch (Exception e)
            {
                logging.Error("Cannot start the application", e);
                Console.ReadKey(); 
            }
        }        
    }
}