using System;
using System.Linq;
using Castle.Windsor;
using Serilog;
using Serilog.Extras.Topshelf;
using Topshelf;

namespace OpenHomeServer.Server
{
    static class Startup
    {
        private static Host _host;

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
                        new Web.NancyInstaller(),
                        new Plugins.PluginInstaller(),
                        new Storage.StorageInstaller()
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
            }
            catch (Exception e)
            {
                logging.Error(e, "Cannot start the application");
            }
        }        
    }
}