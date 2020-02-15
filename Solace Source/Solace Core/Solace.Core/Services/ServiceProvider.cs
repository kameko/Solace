
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
        private ConcurrentDictionary<string, IService> Services { get; set; }
        
        public ServiceProvider()
        {
            OnServiceLoad   = delegate { };
            OnServiceUnload = delegate { };
            Services        = new ConcurrentDictionary<string, IService>();
        }
        
        internal Task HandleModuleUnloading(IEnumerable<string> services)
        {
            return Task.Run(() =>
            {
                foreach (var service_name in services)
                {
                    var success = RemoveService(service_name);
                    if (!success)
                    {
                        Log.Warning($"Attempted to remove service \"{service_name}\" but was not successful");
                    }
                }
            });
        }
        
        internal Task HandleModulesFound(IEnumerable<IService> services)
        {
            return Task.Run(() =>
            {
                foreach (var service in services)
                {
                    try
                    {
                        // TODO: figure out what to do with the cancel token situation
                        // await service.Start();
                    }
                    catch
                    {
                        
                    }
                    var success = AddService(service);
                    if (!success)
                    {
                        Log.Warning($"Attempted to add service \"{service.Name}\" but was not successful");
                    }
                }
            });
        }
        
        internal bool AddService(IService service)
        {
            return Services.TryAdd(service.Name, service);
        }
        
        private bool RemoveService(string name)
        {
            return Services.Remove(name, out var _);
        }
        
        public bool GetService(string service_name, out IService service)
        {
            var ls = new List<IService>(Services.Select(x => x.Value));
            service = ls.Find(x => x.Name.Equals(service_name, StringComparison.InvariantCultureIgnoreCase))!;
            
            if (service is null)
            {
                return false;
            }
            
            return true;
        }
        
        public bool GetService<T>(string service_name, out T service) where T : IService
        {
            IService? temp_service = null;
            var ls = new List<IService>(Services.Select(x => x.Value));
            temp_service = ls.Find(x => x.Name.Equals(service_name, StringComparison.InvariantCultureIgnoreCase))!;
            
            if (!(temp_service is null) && temp_service is T tserv)
            {
                service = tserv;
                return true;
            }
            
            service = default!;
            return false;
        }
        
        public bool GetService<T>(out T service) where T : IService
        {
            IService? temp_service = null;
            var ls = new List<IService>(Services.Select(x => x.Value));
            temp_service = ls.Find(x => x is T)!;
            
            if (!(temp_service is null) && temp_service is T tserv)
            {
                service = tserv;
                return true;
            }
            
            service = default!;
            return false;
        }
    }
}
