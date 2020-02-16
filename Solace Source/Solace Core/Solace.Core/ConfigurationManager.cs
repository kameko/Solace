
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
        public static readonly string DefaultLocation = "./solace.conf";
        
        public event Func<Configuration, Task> OnConfigurationReload;
        public string Location { get; set; }
        
        public ConfigurationManager(string? location)
        {
            OnConfigurationReload = delegate { return Task.CompletedTask; };
            Location              = location ?? DefaultLocation;
        }
        
        public Configuration Load()
        {
            if (File.Exists(Location))
            {
                return LoadAndConvert();
            }
            else
            {
                Log.Info($"No configuration file found at {Location}. Creating default file");
                return CreateDefault();
            }
        }
        
        public async Task Reload()
        {
            await Reload(Location);
        }
        
        public async Task<Configuration> Reload(string new_path)
        {
            Location = new_path;
            var cfg  = Load();
            await OnConfigurationReload.Invoke(cfg);
            return cfg;
        }
        
        public async Task InstallNewValues(Configuration conf)
        {
            var config = Load();
            config.SetValue(conf.Service, conf);
            // TODO: write config
            await OnConfigurationReload.Invoke(config);
        }
        
        private Configuration CreateDefault()
        {
            var cfg = Configuration.GetDefault();
            var opt = new JsonSerializerOptions()
            {
                IgnoreReadOnlyProperties = true,
                ReadCommentHandling      = JsonCommentHandling.Allow,
                WriteIndented            = true,
                AllowTrailingCommas      = true,
            };
            var json = JsonSerializer.Serialize(cfg, opt);
            
            try
            {
                File.WriteAllText(Location, json);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Could not write default configuration file");
            }
            
            return cfg;
        }
        
        private Configuration LoadAndConvert()
        {
            var text = File.ReadAllText(Location);
            var cfg  = JsonSerializer.Deserialize<Configuration>(text);
            return cfg;
        }
    }
}
