
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Sockets;
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
            var reason = string.Empty;
            StreamReader? reader = null;
            try
            {
                reader = new StreamReader(Request.Body);
                reason = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Log.Warning(e, "Attempted to read shutdown request, but failed.");
            }
            finally
            {
                reader?.Dispose();
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
                if (!IsPortOpen(client))
                {
                    var port = client.BaseAddress!.Port;
                    log.Warning("Port {port} is not open.", port);
                    return -1;
                }
                
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
                if (!IsPortOpen(client))
                {
                    var port = client.BaseAddress!.Port;
                    log.Warning("Port {port} is not open.", port);
                    return $"Error: Port {port} is not open.";
                }
                
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
        
        public static bool IsPortOpen(HttpClient client)
        {
            // I hate how I have to do any of this just to see
            // if a port is being used, but, that's the wonderful
            // world of information technology for you.
            
            var baseaddr = client.BaseAddress;
            var host     = baseaddr!.Host;
            var port     = baseaddr!.Port;
            
            TcpClient? tcp = null;
            try
            {
                tcp = new TcpClient();
                var async_result = tcp.BeginConnect(host, port, null, null);
                var success = async_result.AsyncWaitHandle.WaitOne(1_000);
                if (success)
                {
                    tcp.EndConnect(async_result);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                tcp?.Dispose();
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
