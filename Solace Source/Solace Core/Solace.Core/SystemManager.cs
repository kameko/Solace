
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Text.Json;
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
            Modules.Stop();
            Services.StopAllServices();
        }
        
        public async Task InstallModule(string name, string path)
        {
            try
            {
                var conf    = Config.Load();
                var modules = GetModulesConfig(conf);
                modules.Add(name, path);
                Config.WriteConfig(conf);
                
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
        
        private async Task InstallConfig()
        {
            var modules = new Dictionary<string, string>()
            {
                { "[NONE]", "[NONE]" }
            };
            await Config.InstallNewValue("Modules", modules);
        }
        
        private Dictionary<string, string> GetModulesConfig(Configuration conf)
        {
            // TODO: fix this, change Configuration's dictionary
            // to string/IConfiguration instead of string/object
            
            var modules     = new Dictionary<string, string>();
            var modules_elm = conf.GetValue<JsonElement>("Modules");
            foreach (var item in modules_elm.EnumerateObject())
            {
                modules.Add(item.Name, item.Value.GetString());
            }
            return modules;
        }
    }
}
