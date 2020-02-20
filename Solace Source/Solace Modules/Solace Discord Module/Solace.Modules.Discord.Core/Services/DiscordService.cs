
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
    
    // TODO: AssemblyLoadContext may not be able to support both
    // the DiscordService and the Provider depending on DiscordService
    // as indevidual plugins. We will either need to:
    // A: Make them both one single plugin, or
    // B: Make DiscordService a part of the Core project.
    // Further testing and corrispondance with the .NET team is required,
    // as of now this project is stalled.
    
    public class DiscordService : BaseChatService
    {
        private CancellationTokenSource CancelToken { get; set; }
        private ConfigurationManager Config { get; set; }
        private ServiceProvider Services { get; set; }
        private IDiscordProvider? Backend { get; set; }
        
        public DiscordService() : base()
        {
            Config      = null!;
            Services    = null!;
            Backend     = null;
            CancelToken = new CancellationTokenSource();
        }
        
        public override Task Install(ConfigurationManager config)
        {
            var conf = config.Load();
            if (!conf.Configuration.ContainsKey("Discord"))
            {
                Log.Info($"No Discord config present. Writing config section to configuration file. Please update these values");
                
                var dconf = CreateDefaultConfig();
                conf.Configuration.Add("Discord", dconf);
                config.WriteConfig(conf);
            }
            return Task.CompletedTask;
        }
        
        public override Task Setup(ConfigurationManager config, ServiceProvider services)
        {
            Services = services;
            Config   = config;
            return Task.Run(async () =>
            {
                services.OnServiceUnload += OnServiceUnload;
                services.OnServiceLoad   += OnServiceLoad;
                
                // It's okay if this fails. It just means the provider didn't load yet.
                // The system will wait for it to load and then configure it from there.
                var discord_service_success = Services.GetService<IDiscordProvider>(out var provider);
                if (discord_service_success)
                {
                    await CreateBackend(provider);
                }
                else
                {
                    Log.Info($"A Discord provider module is not loaded. Waiting for Discord provider");
                }
            });
        }
        
        public override Task Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                if (CancelToken.IsCancellationRequested)
                {
                    CancelToken = new CancellationTokenSource();
                }
                
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(-1, token);
                }
                
                CancelToken.Cancel();
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
                Log.Info($"Discord provider is being unloaded. All Discord services will be stopped until a new one is loaded");
                
                await DisposeOldBackend();
            }
        }
        
        private async Task OnServiceLoad(IService service)
        {
            if (object.ReferenceEquals(service, this))
            {
                return;
            }
            
            Log.Debug($"Service found. Type of {service.GetType().Name}"
                + $". Service is {(service is IDiscordProvider ? "" : "NOT")} IDiscordProvider"
                // + $". It is of type {}"
            );
            // FIXME: this isn't passing, something is wrong.
            if (service is IDiscordProvider provider && Backend is null)
            {
                Log.Info($"Discord provider \"{service.Name}\" was found. Initializing");
                
                await DisposeOldBackend();
                Backend = provider;
                await CreateBackend(provider);
            }
        }
        
        private async Task CreateBackend(IDiscordProvider provider)
        {
            try
            {
                var dcfg = LoadConfig();
                Backend = provider;
                await Backend.Setup(dcfg);
                await Backend.StartProvider(CancelToken.Token);
            }
            catch (KeyNotFoundException e)
            {
                var _ = e;
                // TODO: handle
            }
        }
        
        private DiscordConfig LoadConfig()
        {
            var cfg  = Config.Load();
            var dcfg = DiscordConfig.FromConfig(cfg);
            return dcfg;
        }
        
        private ConfigurationElement CreateDefaultConfig()
        {
            var conf = new DiscordConfig();
            return conf.ToConfig();
        }
        
        private async Task DisposeOldBackend()
        {
            var backend = Backend;
            Backend     = null;
            if (!(backend is null))
            {
                Log.Info($"Destroying old Discord provider");
                
                await backend.Disconnect();
                await backend.DisposeAsync();
                GC.Collect(); // to help AssemblyLoadContext unload the module.
            }
        }
    }
}
