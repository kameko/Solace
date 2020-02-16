
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
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
            
            Modules  = new ModuleManager(Config);
            Services = new ServiceProvider(Config);
            
            Modules.OnServicesFound       += Services.HandleModulesFound;
            Modules.OnRequestStopServices += Services.HandleModuleUnloading;
        }
        
        public async Task Setup()
        {
            await InstallConfig();
        }
        
        public void Start()
        {
            Modules.Start();
        }
        
        public async Task InstallModule(string name, string path)
        {
            try
            {
                var conf    = Config.Load();
                var sysconf = conf.GetValue<Configuration>("System");
                var modules = sysconf.GetValue<Dictionary<string,string>>("Modules");
                modules.Add(name, path);
                Config.WriteConfig(conf);
            }
            catch
            {
                // TODO: handle error
            }
            
            await Modules.Load(name, path);
        }
        
        private async Task InstallConfig()
        {
            var conf = new Configuration("System");
            conf.SetValue("Modules", new Dictionary<string, string>());
            await Config.InstallNewValues(conf);
        }
    }
}
