
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Modules;
    using Services;
    
    public class SystemManager
    {
        private ModuleManager Modules { get; set; }
        private ServiceProvider Services { get; set; }
        
        public SystemManager()
        {
            Modules  = new ModuleManager();
            Services = new ServiceProvider();
            
            Modules.OnRequestStopServices += Services.HandleModuleUnloading;
        }
    }
}
