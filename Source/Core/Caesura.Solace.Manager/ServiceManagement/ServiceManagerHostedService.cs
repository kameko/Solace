
namespace Caesura.Solace.Manager.ServiceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.ComponentModel;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Foundation;
    using Foundation.Logging;
    using Foundation.ConfigurationModels;
    
    public class ServiceManagerHostedService : IHostedService, IDisposable
    {
        private readonly ILogger Log;
        private readonly IConfiguration Configuration;
        private readonly ServicesModel ConfiguredServicesModel;
        private readonly ISolaceServiceCollection Services;
        private readonly List<ServiceSession> Sessions;
        
        public ServiceManagerHostedService(ILogger<ServiceManagerHostedService> log, IConfiguration config, ISolaceServiceCollection services)
        {
            Log                     = log;
            Configuration           = config;
            ConfiguredServicesModel = Configuration.GetSection(ConfigurationConstants.Services).Get<ServicesModel>();
            Services                = services;
            
            Sessions                = new List<ServiceSession>();
            
            LifetimeEventsHostedService.OnStopping += Autoclose;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var (name, model) in ConfiguredServicesModel.Items)
            {
                var client_result = Services.TryGet(name, out var client);
                if (!client_result)
                {
                    Log.Error("Client {name} not found in ServiceCollection.", name);
                    continue;
                }
                var session_result = ServiceSession.TryCreate(name, client!, model, out var session);
                if (ServiceSession.IsValid(session_result) && !(session is null))
                {
                    Sessions.Add(session);
                    Log.Debug("Loaded model {name} {model}", name, model);
                }
                else
                {
                    Log.Error("Problem {result} loading service configuration {name} from model {model}",
                        session_result, name, model
                    );
                }
            }
            
            Task.Run(() => RunAsync(cancellationToken));
            
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            
            return Task.CompletedTask;
        }
        
        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(3_000);
                
                while (!cancellationToken.IsCancellationRequested)
                {
                    foreach (var session in Sessions)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        
                        if (!(session.Handle is null))
                        {
                            try
                            {
                                if (!session.Handle.HasExited)
                                {
                                    continue;
                                }
                            }
                            catch (Exception e)
                            {
                                session.Handle = null;
                                Log.Error(e, "Error checking process handle for service {name}.", session.Name);
                            }
                        }
                        
                        if (session.Local)
                        {
                            await HandleLocalSession(session);
                        }
                        else
                        {
                            await HandleRemoteSession(session);
                        }
                    }
                    
                    await Task.Delay(ConfiguredServicesModel.ReconnectDelayMs);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, nameof(RunAsync));
            }
        }
        
        private void Autoclose()
        {
            var reqcount = Sessions.FindAll(x => x.Autoclose).Count();
            if (reqcount > 0)
            {
                Log.Information(
                    "Requesting {number} service"
                  + (reqcount > 1 ? "s" : string.Empty)
                  + " to close.",
                    reqcount
                );
            }
            
            foreach (var session in Sessions)
            {
                if (session.Autoclose)
                {
                    Log.Information("Shutting down service {name}.", session.Name);
                    
                    if (session.Local && session.Handle is null)
                    {
                        Log.Warning(
                            "Local session {name} process handle is null. "
                          + "Trying to send shutdown request, anyway.", 
                            session.Name
                        );
                    }
                    
                    session.Client.RequestShutdown("System Shutdown Initiated.");
                }
            }
        }
        
        private async Task HandleLocalSession(ServiceSession session)
        {
            var pid = await TryContactSession(session);
            if (pid <= 0)
            {
                await HandleNonstartedLocalSession(session);
            }
            else
            {
                await HandleRunningLocalSession(pid, session);
            }
        }
        
        private Task HandleNonstartedLocalSession(ServiceSession session)
        {
            if (!session.Autostart)
            {
                Log.Information("Service {name} is not set to autostart. Ignoring.", session.Name);
                return Task.CompletedTask;
            }
            
            Process? proc = null;
            try
            {
                Log.Information("Start service {name}", session.Name);
                proc = new Process();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.FileName        = session.ExecutablePath.FullName;
                proc.StartInfo.CreateNoWindow  = !session.CreateWindow;
                // TODO: consider redirecting this to this process.
                // proc.StartInfo.RedirectStandardOutput
                proc.Start();
                session.Handle = proc;
                Log.Information("Started service {name}", session.Name);
            }
            catch (Win32Exception w32e)
            {
                OnAnyException(w32e);
            }
            catch (ObjectDisposedException ode)
            {
                OnAnyException(ode);
            }
            catch (Exception e)
            {
                OnAnyException(e);
                throw;
            }
            
            return Task.CompletedTask;
            
            void OnAnyException(Exception e)
            {
                Log.Error(e, "Error in non-running service {name}. Cleaning up resources.", session.Name);
                session.Handle = null;
                proc?.Dispose();
            }
        }
        
        private Task HandleRunningLocalSession(int pid, ServiceSession session)
        {
            try
            {
                var proc = Process.GetProcessById(pid);
                if (proc.HasExited)
                {
                    throw new InvalidOperationException("Process has excited.");
                }
                session.Handle = proc;
            }
            catch (Win32Exception w32e)
            {
                OnAnyException(w32e);
            }
            catch (InvalidOperationException ioe)
            {
                OnAnyException(ioe);
            }
            catch (Exception e)
            {
                OnAnyException(e);
                throw;
            }
            
            return Task.CompletedTask;
            
            void OnAnyException(Exception e)
            {
                Log.Error(e, "Error in running service {name}. Cleaning up resources.", session.Name);
                session.Handle = null;
            }
        }
        
        private async Task HandleRemoteSession(ServiceSession session)
        {
            // TODO: we'll have to add some kind of remote watchdog system
            // that runs on the other server to check for messages sent 
            // from here to start and stop remote services.
            var pid = await TryContactSession(session);
            if (pid <= 0)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        
        // ...
        
        private Task<int> TryContactSession(ServiceSession session)
        {
            return session.Client.RequestPid();
        }
        
        public void Dispose()
        {
            Log.Trace("Disposing.");
            foreach (var session in Sessions)
            {
                session.Dispose();
            }
        }
    }
}
