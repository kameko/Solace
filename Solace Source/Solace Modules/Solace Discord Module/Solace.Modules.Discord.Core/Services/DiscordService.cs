
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
        
        public override async Task Install(ConfigurationManager config)
        {
            var conf = CreateDefaultConfig();
            await config.InstallNewValues(conf);
        }
        
        public override Task Setup(ConfigurationManager config, ServiceProvider services)
        {
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
            var cfg  = Config.Load();
            var ccfg = cfg.GetValue<Configuration>("Discord");
            var dcfg = new DiscordConfig()
            {
                ConnectionToken = ccfg.GetValue<string>("ConnectionToken"),
                DebugLog        = ccfg.GetValue<bool>("DebugLog"),
                PingTimeout     = ccfg.GetValue<int>("PingTimeout"),
                PingTries       = ccfg.GetValue<int>("PingTries"),
                LogLevel        = ccfg.GetValue<Log.LogLevel>("LogLevel"),
            };
            return dcfg;
        }
        
        private Configuration CreateDefaultConfig()
        {
            var conf = new Configuration("Discord");
            conf.SetValue("ConnectionToken", "[NONE]");
            conf.SetValue("DebugLog", true);
            conf.SetValue("PingTimeout", 5000);
            conf.SetValue("PingTries", 3);
            conf.SetValue("LogLevel", Log.LogLevel.Info);
            return conf;
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
