
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;
    using System.Text.Json;
    
    public class ConfigurationManager
    {
        public event Func<IConfiguration, Task> OnConfigurationReload;
        public string Location { get; set; }
        
        public ConfigurationManager(string location)
        {
            OnConfigurationReload = delegate { return Task.CompletedTask; };
            Location              = location;
        }
        
        public IConfiguration Load()
        {
            return default!;
        }
        
        public async Task Reload()
        {
            await Reload(Location);
        }
        
        public async Task<IConfiguration> Reload(string new_path)
        {
            Location = new_path;
            var cfg = Load();
            await OnConfigurationReload.Invoke(cfg);
            return cfg;
        }
    }
}
