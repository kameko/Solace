
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Logging;
    using ConfigurationModels;
    
    public abstract class ProcControllerBase : ControllerBase
    {
        protected ILogger Log { get; }
        protected IConfiguration Configuration { get; }
        
        protected bool AllowShutdown { get; }
        protected int ShutdownDelay { get; }
        
        internal ProcControllerBase(ILogger logger, IConfiguration configuration)
        {
            Log           = logger;
            Configuration = configuration;
            
            var model     = Configuration.GetSection(ConfigurationConstants.Process).Get<ProcModel>();
            AllowShutdown = model.AllowShutdown;
            ShutdownDelay = model.ShutdownDelayMs;
        }
        
        // TODO: GetStatus, return an enum, or maybe a string. Probably gonna need it
        // from a static hosted service or something. Maybe put it in Lifetime.
        
        [HttpGet("pid")]
        public virtual int GetPid()
        {
            Log.EnterMethod(nameof(GetPid));
            var pid = Process.GetCurrentProcess().Id;
            Log.Information("GET request for current Process ID. Returning {pid}.", pid);
            Log.ExitMethod(nameof(GetPid));
            return pid;
        }
        
        [HttpPost("shutdown")]
        public virtual string PostShutdown()
        {
            Log.EnterMethod(nameof(PostShutdown));
            
            var reason = HttpHelper.RequestBodyAsString(Request);
            if (reason is null)
            {
                Log.Warning("Attempted to read shutdown request, but failed. Continuing anyway.");
            }
            
            if (AllowShutdown)
            {
                if (string.IsNullOrEmpty(reason))
                {
                    Log.Information(
                        "Shutdown request encountered. Service will shut down in {delay} milliseconds. "
                      + "No reason given.",
                        ShutdownDelay
                    );
                }
                else
                {
                    Log.Information(
                        "Shutdown request encountered. Service will shut down in {delay} milliseconds."
                      + "Reason: {reason}",
                        ShutdownDelay,
                        reason
                    );
                }
                
                Task.Run(async () =>
                {
                    await Task.Delay(ShutdownDelay);
                    Log.Information("Countdown complete. Now requesting service shutdown.");
                    LifetimeEventsHostedService.StopApplication();
                });
                
                Log.ExitMethod(nameof(PostShutdown));
                return $"OK. Service will shut down in {ShutdownDelay} milliseconds.";
            }
            else
            {
                Log.Information("Request to shut down current service. Shutdown is disabled, request denied.");
                Log.ExitMethod(nameof(PostShutdown));
                return "Shutdown not enabled.";
            }
        }
        
        public static async Task<int> RequestPid(ILogger log, HttpClient client, CancellationToken token)
        {
            log.EnterMethod(nameof(RequestPid));
            try
            {
                //*
                if (!HttpHelper.IsPortOpen(client))
                {
                    var port = client.BaseAddress!.Port;
                    log.Warning("Port {port} is not open.", port);
                    return -1;
                }
                //*/
                
                var response = await client.GetAsync("/proc/pid", token);

                response.EnsureSuccessStatusCode();

                var responseStr = await response.Content!.ReadAsStringAsync();
                if (int.TryParse(responseStr, out int pid))
                {
                    return pid;
                }
                else
                {
                    log.Debug("PID request return value not an integer. Value: {value}.", responseStr);
                    return -1;
                }
            }
            catch (HttpRequestException e)
            {
                log.Debug(e, nameof(RequestPid));
                return -1;
            }
            finally
            {
                log.ExitMethod(nameof(RequestPid));
            }
        }
        
        public static async Task<string> RequestShutdown(string reason, ILogger log, HttpClient client, CancellationToken token)
        {
            log.EnterMethod(nameof(RequestShutdown));
            try
            {
                //*
                if (!HttpHelper.IsPortOpen(client))
                {
                    var port = client.BaseAddress!.Port;
                    log.Warning("Port {port} is not open.", port);
                    return $"Error: Port {port} is not open.";
                }
                //*/
                
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
            catch (HttpRequestException e)
            {
                log.Debug(e, nameof(RequestShutdown));
                return string.Empty;
            }
            finally
            {
                log.ExitMethod(nameof(RequestShutdown));
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
