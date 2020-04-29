
namespace Caesura.Solace.Manager
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Foundation;
    using Foundation.Logging;
    
    // TODO: Add a ServiceProcessManagerService, like
    // LifetimeEventsHostedService. It will be responsible
    // for starting up and tracking all services.
    
    public class Program
    {
        public static void Main(string[] args)
        {
            using (WindowTitle.Set("Caesura Solace Manager"))
            {
                CreateHostBuilder(args).Build().Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging
                        .ClearProviders()
                        .AddSolaceConsoleLogger(config =>
                        {
                            config.LogLevel  = LogLevel.Trace;
                            config.TrimNames = new List<string>()
                            {
                                "Caesura.Solace.Manager.Controllers.",
                            };
                        });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<LifetimeEventsHostedService>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            
            return host;
        }
    }
}
