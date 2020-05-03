
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Logging;
    
    public abstract class ProcControllerBase
    {
        protected ILogger Log { get; }
        protected IConfiguration Configuration { get; }
        
        protected bool AllowShutdown { get; }
        protected int ShutdownDelay { get; }
        
        internal ProcControllerBase(ILogger logger, IConfiguration configuration)
        {
            Log           = logger;
            Configuration = configuration;
            
            AllowShutdown = true;
            if (bool.TryParse(Configuration["Proc:AllowShutdown"], out var allow_shutdown))
            {
                AllowShutdown = allow_shutdown;
            }
            
            ShutdownDelay = 3_000;
            if (int.TryParse(Configuration["Proc:ShutdownDelay"], out var shutdown_delay) && shutdown_delay > 0)
            {
                ShutdownDelay = shutdown_delay;
            }
        }
        
        [HttpGet("pid")]
        public virtual int GetPid()
        {
            var pid = Process.GetCurrentProcess().Id;
            Log.Information("GET request for current Process ID. Returning {pid}.", pid);
            return pid;
        }
        
        [HttpPost("shutdown")]
        public virtual string PostShutdown()
        {
            // TODO: figure out how to get the body.
            if (AllowShutdown)
            {
                Log.Information("Shutdown request encountered. Service will shut down in {delay} milliseconds.", ShutdownDelay);
                
                Task.Run(async () =>
                {
                    await Task.Delay(ShutdownDelay);
                    LifetimeEventsHostedService.StopApplication();
                });
                
                return $"OK. Service will shut down in {ShutdownDelay} milliseconds.";
            }
            else
            {
                Log.Information("Request to shut down current service. Shutdown is disabled, request denied.");
                return "Shutdown not enabled.";
            }
        }
        
        public static async Task<int> RequestPid(HttpClient client, CancellationToken token)
        {
            try
            {
                var response = await client.GetAsync("/proc/pid", token);

                response.EnsureSuccessStatusCode();

                var responseStr = await response.Content!.ReadAsStringAsync();
                if (int.TryParse(responseStr, out int pid))
                {
                    return pid;
                }
                return -1;
            }
            catch (HttpRequestException)
            {
                return -1;
            }
        }
        
        public static async Task<string> RequestShutdown(string reason, HttpClient client, CancellationToken token)
        {
            try
            {
                var response = await client.PostAsync(
                    $"proc/shutdown", 
                    new StringContent(
                        reason,
                        Encoding.UTF8,
                        "text/plain"),
                    token
                );
                
                response.EnsureSuccessStatusCode();

                var responseStr = await response.Content!.ReadAsStringAsync();
                return responseStr;
            }
            catch (HttpRequestException)
            {
                return string.Empty;
            }
        }
    }
    
    public abstract class ProcControllerBase<T> : ProcControllerBase
    {
        public ProcControllerBase(ILogger<T> logger, IConfiguration configuration)
            : base(logger, configuration)
        {
            
        }
    }
}
