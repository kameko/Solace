
namespace Solace.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ServiceProvider
    {
        public event Action<IService> OnServiceLoad;
        public event Action<string> OnServiceUnload;
        
        private List<IService> Services { get; set; }
        // TODO: replace lock with something else, either a concurrent
        // collection or SemaphoreSlim.WaitAsync()
        private readonly object ServicesLock = new object();
        
        public ServiceProvider()
        {
            OnServiceLoad   = delegate { };
            OnServiceUnload = delegate { };
            Services        = new List<IService>();
        }
        
        internal void AddService(IService service)
        {
            lock (ServicesLock)
            {
                Services.Add(service);
            }
        }
        
        public bool GetService(string service_name, out IService service)
        {
            lock (ServicesLock)
            {
                service = Services.Find(x => x.Name.Equals(service_name, StringComparison.InvariantCultureIgnoreCase))!;
            }
            
            if (service is null)
            {
                return false;
            }
            
            return true;
        }
        
        public bool GetService<T>(string service_name, out T service) where T : IService
        {
            IService? temp_service = null;
            lock (ServicesLock)
            {
                temp_service = Services.Find(x => x.Name.Equals(service_name, StringComparison.InvariantCultureIgnoreCase))!;
            }
            
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
            lock (ServicesLock)
            {
                temp_service = Services.Find(x => x is T)!;
            }
            
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
