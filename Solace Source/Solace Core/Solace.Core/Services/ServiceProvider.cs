
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
        private ConcurrentBag<IService> Services { get; set; }
        
        public ServiceProvider()
        {
            OnServiceLoad   = delegate { };
            OnServiceUnload = delegate { };
            Services        = new ConcurrentBag<IService>();
        }
        
        internal Task HandleModuleUnloading(IEnumerable<string> services)
        {
            throw new NotImplementedException();
        }
        
        internal void AddService(IService service)
        {
            Services.Add(service);
        }
        
        public bool GetService(string service_name, out IService service)
        {
            var ls = new List<IService>(Services);
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
            var ls = new List<IService>(Services);
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
            var ls = new List<IService>(Services);
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
