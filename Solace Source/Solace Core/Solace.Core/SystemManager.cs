
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Modules;
    using Services;
    
    // TODO: need to handle reloading configs
    
    public class SystemManager
    {
        private ConfigurationManager Config { get; set; }
        private ModuleManager Modules { get; set; }
        private ServiceProvider Services { get; set; }
        
        public SystemManager(string config_location)
        {
            Config   = new ConfigurationManager(config_location);
            var cfg  = Config.Load(); 
            
            Modules  = new ModuleManager();
            Services = new ServiceProvider(cfg);
            
            Modules.OnServicesFound       += Services.HandleModulesFound;
            Modules.OnRequestStopServices += Services.HandleModuleUnloading;
        }
    }
}
