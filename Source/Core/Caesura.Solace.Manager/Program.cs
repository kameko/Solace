
namespace Caesura.Solace.Manager
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Foundation;
    using Foundation.Logging;
    
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO: move this elsewhere, let the config handle this.
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
                                "System.Net.Http.",
                            };
                        });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            
            return host;
        }
    }
}
