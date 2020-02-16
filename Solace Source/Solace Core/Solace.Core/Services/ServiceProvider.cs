
namespace Solace.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public class ServiceProvider
    {
        public event Action<IService> OnServiceLoad;
        public event Action<string> OnServiceUnload;
        
        private IConfiguration Config { get; set; }
        private readonly object ServicesLock = new object();
        private List<ServiceContainer> Services { get; set; }
        private Dictionary<string, CancellationTokenSource> ModuleCancelTokens { get; set; }
        
        public ServiceProvider(IConfiguration config)
        {
            OnServiceLoad      = delegate { };
            OnServiceUnload    = delegate { };
            
            Config             = config;
            Services           = new List<ServiceContainer>();
            ModuleCancelTokens = new Dictionary<string, CancellationTokenSource>();
        }
        
        internal Task HandleModuleUnloading(string module_name, IEnumerable<string> services)
        {
            return Task.Run(() =>
            {
                lock (ServicesLock)
                {
                    var token = ModuleCancelTokens[module_name];
                    token.Cancel();
                    ModuleCancelTokens.Remove(module_name);
                    
                    foreach (var service_name in services)
                    {
                        var success = RemoveService(service_name);
                        if (!success)
                        {
                            Log.Warning($"Attempted to remove service \"{service_name}\" but was not successful");
                        }
                    }
                }
            });
        }
        
        internal Task HandleModulesFound(string module_name, IEnumerable<IService> services)
        {
            return Task.Run(async () =>
            {
                CancellationTokenSource token = null!;
                lock (ServicesLock)
                {
                    if (ModuleCancelTokens.ContainsKey(module_name))
                    {
                        token = ModuleCancelTokens[module_name];
                    }
                    else
                    {
                        token = new CancellationTokenSource();
                        ModuleCancelTokens.Add(module_name, token);
                    }
                    
                    foreach (var service in services)
                    {
                        AddService(module_name, service);
                    }
                }
                
                foreach (var service in services)
                {
                    try
                    {
                        await service.Setup(Config, this);
                        await service.Start(token.Token);
                    }
                    catch
                    {
                        // TODO: handle error
                    }
                }
            });
        }
        
        private void AddService(string module_name, IService service)
        {
            lock (ServicesLock)
            {
                var container = new ServiceContainer(module_name, service);
                Services.Add(container);
            }
        }
        
        private bool RemoveService(string name)
        {
            lock (ServicesLock)
            {
                var container = Services.Find(x => x.ServiceName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (container is null)
                {
                    return false;
                }
                return Services.Remove(container);
            }
        }
        
        public bool GetService(string service_name, out IService service)
        {
            lock (ServicesLock)
            {
                var container = Services.Find(x => x.ServiceName.Equals(service_name, StringComparison.InvariantCultureIgnoreCase));
                if (container is null)
                {
                    service = null!;
                    return false;
                }
                service = container.Service;
                return true;
            }
        }
        
        public bool GetService<T>(string service_name, out T service) where T : IService
        {
            lock (ServicesLock)
            {
                var container = Services.Find(x => x.ServiceName.Equals(service_name, StringComparison.InvariantCultureIgnoreCase));
                if (container is T ts)
                {
                    service = ts;
                    return true;
                }
                service = default!;
                return false;
            }
        }
        
        public bool GetService<T>(out T service) where T : IService
        {
            lock (ServicesLock)
            {
                var container = Services.Find(x => x.Service is T);
                if (container is null)
                {
                    service = default!;
                    return false;
                }
                service = (T)container.Service;
                return true;
            }
        }
        
        private class ServiceContainer
        {
            public IService Service { get; set; }
            public string ServiceName => Service.Name;
            public string ModuleName { get; set; }
            
            public ServiceContainer(string module_name, IService service)
            {
                Service    = service;
                ModuleName = module_name;
            }
        }
    }
}
