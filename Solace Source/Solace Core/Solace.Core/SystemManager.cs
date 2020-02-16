
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
            InstallConfig();
            
            Modules  = new ModuleManager();
            Services = new ServiceProvider(Config);
            
            Modules.OnServicesFound       += Services.HandleModulesFound;
            Modules.OnRequestStopServices += Services.HandleModuleUnloading;
        }
        
        private void InstallConfig()
        {
            Task.Run(async () =>
            {
                var conf = new Configuration();
                conf.SetValue("Services", new List<string>());
                await Config.InstallNewValues(conf);
            });
        }
    }
}
