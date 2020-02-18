
namespace Solace.Modules.Discord.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Solace.Core;
    using Solace.Core.Services;
    using Solace.Core.Services.Communication;
    using Providers;
    
    // TODO: Use the Heartbeat callback from the Provider here to
    // check a configured amount of heartbeats missed. If enough has
    // not been fired. perform the following:
    // - Check if there is an internet connection
    // - Ping Discord
    // - Reconnect
    // If any step fails, wait a few seconds and restart the cycle.
    // Configuration items required:
    // - HeartbeatsSkipped (int, default 5)
    // - PingUrls (List<string>, default { discordapp.com })
    // - RetryReconnectDelay (int, default 5000)
    
    public class DiscordService : BaseChatService
    {
        private ConfigurationManager Config { get; set; }
        private ServiceProvider Services { get; set; }
        private IDiscordProvider? Backend { get; set; }
        
        public DiscordService() : base()
        {
            Config   = null!;
            Services = null!;
            Backend  = null;
        }
        
        public override Task Install(ConfigurationManager config)
        {
            var conf = config.Load();
            if (!conf.Configuration.ContainsKey("Discord"))
            {
                var dconf = CreateDefaultConfig();
                conf.Configuration.Add("Discord", dconf);
            }
            return Task.CompletedTask;
        }
        
        public override Task Setup(ConfigurationManager config, ServiceProvider services)
        {
            // TODO: check for DiscordConfig from the config manager. If not present,
            // wait until it becomes present. We're gonna buffer config writes so this
            // may get called before Install finishes updating the config to have it's
            // config values.
            // Also, after it installs it's config values, they'll be default, so wait
            // here until the config is updated so we can get user-configured values
            // at runtime.
            
            Services = services;
            Config   = config;
            return Task.Run(() =>
            {
                services.OnServiceUnload += OnServiceUnload;
                services.OnServiceLoad   += OnServiceLoad;
                
                // It's okay if this fails. It just means the provider didn't load yet.
                // The system will wait for it to load and then configure it from there.
                var discord_service_success = Services.GetService<IDiscordProvider>(out var provider);
                if (discord_service_success)
                {
                    CreateBackend(provider);
                }
            });
        }
        
        public override Task Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(500, token);
                    if (!(Backend is null))
                    {
                        await Backend.Start(token);
                        break;
                    }
                }
            });
            
            return Task.CompletedTask;
        }
        
        public override async Task<bool> Reconnect()
        {
            if (!(Backend is null))
            {
                var success = await Backend.Disconnect();
                if (success)
                {
                    await Task.Delay(1000);
                    return await Backend.Connect();
                }
            }
            return false;
        }
        
        private async Task OnServiceUnload(string service_name)
        {
            if (service_name == (Backend?.Name ?? string.Empty))
            {
                await DisposeOldBackend();
            }
        }
        
        private async Task OnServiceLoad(IService service)
        {
            if (service is IDiscordProvider provider)
            {
                await DisposeOldBackend();
                Backend = provider;
                CreateBackend(provider);
            }
        }
        
        private void CreateBackend(IDiscordProvider provider)
        {
            try
            {
                var dcfg = CreateConfig();
                Backend = provider;
                Backend.Setup(dcfg);
            }
            catch (KeyNotFoundException e)
            {
                var _ = e;
                // TODO: handle
            }
        }
        
        private DiscordConfig CreateConfig()
        {
            /*
            var cfg  = Config.Load();
            var ccfg = cfg.GetValue<ConfigurationToken>("Discord");
            var dcfg = new DiscordConfig()
            {
                ConnectionToken = ccfg.GetValue<string>("ConnectionToken"),
                DebugLog        = ccfg.GetValue<bool>("DebugLog"),
                PingTimeout     = ccfg.GetValue<int>("PingTimeout"),
                PingTries       = ccfg.GetValue<int>("PingTries"),
                LogLevel        = ccfg.GetValue<Log.LogLevel>("LogLevel"),
            };
            return dcfg;
            */
            throw new NotImplementedException();
        }
        
        private ConfigurationElement CreateDefaultConfig()
        {
            // var conf = new ConfigurationToken("Discord");
            /*
            conf.SetValue("ConnectionToken", "[NONE]");
            conf.SetValue("DebugLog", true);
            conf.SetValue("PingTimeout", 5000);
            conf.SetValue("PingTries", 3);
            conf.SetValue("LogLevel", Log.LogLevel.Info);
            */
            // return conf;
            return default!;
        }
        
        private async Task DisposeOldBackend()
        {
            var backend = Backend;
            Backend     = null;
            if (!(backend is null))
            {
                await backend.Disconnect();
                await backend.DisposeAsync();
                GC.Collect(); // to help AssemblyLoadContext unload the module.
            }
        }
    }
}
