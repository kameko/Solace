
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
        
        public SystemManager(string? config_location)
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
            try
            {
                var conf    = Config.Load();
                var modules = GetModulesConfig(conf);
                if (modules is null)
                {
                    throw new NullReferenceException("Modules is null");
                }
                Modules.Start(modules);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error loading modules from configuration file");
            }
        }
        
        public void Stop()
        {
            Config.Start();
            Modules.Stop();
            Services.StopAllServices();
            Config.Stop();
        }
        
        public async Task InstallModule(string name, string path)
        {
            try
            {
                var conf    = Config.Load();
                var modules = GetModulesConfig(conf);
                modules.Add(name, path);
                Config.SaveConfig(conf);
                
                await Modules.Load(name, path);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error installing module \"{name}\" from {path}");
            }
        }
        
        public async Task ReloadModule(string name)
        {
            await Modules.Reload(name);
        }
        
        private Task InstallConfig()
        {
            var conf = Config.Load();
            if (!conf.Configuration.ContainsKey("Modules"))
            {
                var modconf = new ConfigurationElement();
                modconf.Configuration.Add("[NONE]", "[NONE]");
                conf.Configuration.Add("Modules", modconf);
                Config.SaveConfig(conf);
            }
            return Task.CompletedTask;
        }
        
        private Dictionary<string, string> GetModulesConfig(ConfigurationToken conf)
        {
            var modules = conf.Configuration["Modules"].Configuration
                .Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString()));
            return new Dictionary<string, string>(modules);
        }
    }
}
