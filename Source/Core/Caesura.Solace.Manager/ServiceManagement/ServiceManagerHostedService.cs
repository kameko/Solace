
namespace Caesura.Solace.Manager.ServiceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Foundation.Logging;
    using Foundation.ConfigurationModels;
    
    public class ServiceManagerHostedService : IHostedService
    {
        private readonly ILogger Log;
        private readonly IConfiguration Configuration;
        private readonly ServicesModel ConfiguredServicesModel;
        private readonly ServiceCollection Services;
        private readonly List<ServiceSession> Sessions;
        
        public ServiceManagerHostedService(ILogger<ServiceManagerHostedService> log, IConfiguration config, ServiceCollection services)
        {
            Log = log;
            Configuration = config;
            ConfiguredServicesModel = Configuration.GetSection("Services").Get<ServicesModel>();
            Services = services;
            
            Sessions = new List<ServiceSession>();
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
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(15);
                    
                    foreach (var session in Sessions)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
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
                }
            }
            catch (Exception e)
            {
                Log.Error(e, nameof(RunAsync));
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
                Log.Debug("Service {name} is not set to autostart. Ignoring.", session.Name);
                return Task.CompletedTask;
            }
            
            throw new NotImplementedException();
        }
        
        private Task HandleRunningLocalSession(int pid, ServiceSession session)
        {
            throw new NotImplementedException();
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
    }
}
