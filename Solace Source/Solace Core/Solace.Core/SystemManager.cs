
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
        private ConfigurationManager Config { get; set; }
        private ModuleManager Modules { get; set; }
        private ServiceProvider Services { get; set; }
        
        public SystemManager(string config_location)
        {
            Config   = new ConfigurationManager(config_location);
            
            Modules  = new ModuleManager();
            Services = new ServiceProvider(Config);
            
            Modules.OnServicesFound       += Services.HandleModulesFound;
            Modules.OnRequestStopServices += Services.HandleModuleUnloading;
        }
    }
}
