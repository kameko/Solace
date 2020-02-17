
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;
    using Newtonsoft.Json;
    
    public class ConfigurationManager
    {
        public static readonly string DefaultLocation = "./solace.conf";
        
        public event Func<ConfigurationToken, Task> OnConfigurationReload;
        public string Location { get; set; }
        
        public ConfigurationManager(string? location)
        {
            OnConfigurationReload = delegate { return Task.CompletedTask; };
            Location              = location ?? DefaultLocation;
        }
        
        public ConfigurationToken Load()
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
        
        public async Task<ConfigurationToken> Reload(string new_path)
        {
            Location = new_path;
            var cfg  = Load();
            await OnConfigurationReload.Invoke(cfg);
            return cfg;
        }
        
        public void WriteConfig(ConfigurationToken config)
        {
            // TODO: this isn't thread-safe. queue all requests to this
            // and work on them one-by-one so nobody overwrites each other.
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            
            try
            {
                File.WriteAllText(Location, json);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Could not write configuration file");
            }
        }
        
        private ConfigurationToken CreateDefault()
        {
            var cfg = ConfigurationToken.GetDefault();
            WriteConfig(cfg);
            return cfg;
        }
        
        private ConfigurationToken LoadAndConvert()
        {
            // TODO: cache config
            var text = File.ReadAllText(Location);
            var cfg  = JsonConvert.DeserializeObject<ConfigurationToken>(text, JsonOptions())!;
            return cfg;
        }
        
        private JsonSerializerSettings JsonOptions()
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
            return settings;
        }
    }
}
