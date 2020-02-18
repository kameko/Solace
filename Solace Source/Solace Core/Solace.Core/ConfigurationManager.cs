
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Channels;
    using System.IO;
    using Newtonsoft.Json;
    
    public class ConfigurationManager
    {
        public static readonly string DefaultLocation = "./solace.conf";
        
        public event Func<ConfigurationToken, Task> OnConfigurationReload;
        public string Location { get; set; }
        
        private Channel<ConfigurationToken> ConfigBuffer { get; set; }
        private CancellationTokenSource CancelToken { get; set; }
        
        public ConfigurationManager(string? location)
        {
            OnConfigurationReload = delegate { return Task.CompletedTask; };
            Location              = location ?? DefaultLocation;
            ConfigBuffer          = Channel.CreateUnbounded<ConfigurationToken>();
            CancelToken           = null!;
        }
        
        public void Stop()
        {
            CancelToken?.Cancel();
        }
        
        public void Start()
        {
            var token = new CancellationTokenSource();
            CancelToken = token;
            Start(token.Token);
        }
        
        public void Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                // TODO: try to figure out if two configs being written exclude each other's
                // service configs and combine them instead of overwriting each other.
                while (!token.IsCancellationRequested)
                {
                    var success = await ConfigBuffer.Reader.WaitToReadAsync(token);
                    if (success)
                    {
                        var config = await ConfigBuffer.Reader.ReadAsync(token);
                        SaveConfig(config);
                    }
                }
            });
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
        
        public bool WriteConfig(ConfigurationToken config)
        {
            return ConfigBuffer.Writer.TryWrite(config);
        }
        
        internal void SaveConfig(ConfigurationToken config)
        {
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
            SaveConfig(cfg);
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
