
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
    using ConfigurationModels;
    
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
                var session_result = ServiceSession.TryCreate(name, model, out var session);
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
                            HandleLocalSession(session);
                        }
                        else
                        {
                            HandleRemoteSession(session);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, nameof(RunAsync));
            }
        }
        
        private void HandleLocalSession(ServiceSession session)
        {
            var pid = TryContactSession(session);
            if (pid <= 0)
            {
                HandleNonstartedLocalSession(session);
            }
            else
            {
                HandleRunningLocalSession(pid, session);
            }
        }
        
        private void HandleNonstartedLocalSession(ServiceSession session)
        {
            throw new NotImplementedException();
        }
        
        private void HandleRunningLocalSession(int pid, ServiceSession session)
        {
            throw new NotImplementedException();
        }
        
        private void HandleRemoteSession(ServiceSession session)
        {
            // TODO: we'll have to add some kind of remote watchdog system
            // that runs on the other server to check for messages sent 
            // from here to start and stop remote services.
            var pid = TryContactSession(session);
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
        
        private int TryContactSession(ServiceSession session)
        {
            // TODO: Send a GET request to the (future) PidController,
            // if no response, return -1;
            throw new NotImplementedException();
        }
    }
}
