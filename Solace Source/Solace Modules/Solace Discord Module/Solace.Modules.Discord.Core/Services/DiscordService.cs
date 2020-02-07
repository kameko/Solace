
namespace Solace.Modules.Discord.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core;
    using Solace.Core.Services;
    using Solace.Core.Services.Communication;
    using Providers;
    
    public class DiscordService : BaseChatService
    {
        private const string CONFIG_DISCORD_TOKEN = "DISCORD.TOKEN";
        
        private ServiceProvider Services { get; set; }
        private IDiscordProvider? Backend { get; set; }
        private string DiscordToken { get; set; }
        
        public DiscordService() : base()
        {
            Services     = null!;
            Backend      = null;
            DiscordToken = string.Empty;
        }
        
        public override IEnumerable<string> GetAllRequiredConfigurationTokens()
        {
            return new List<string>()
            {
                CONFIG_DISCORD_TOKEN,
            };
        }
        
        public override Task Setup(IConfiguration config, ServiceProvider services)
        {
            Services = services;
            return Task.Run(() =>
            {
                services.OnServiceUnload += OnServiceUnload;
                services.OnServiceLoad   += OnServiceLoad;
                
                var token = config.GetValue(CONFIG_DISCORD_TOKEN);
                if (!string.IsNullOrEmpty(token))
                {
                    DiscordToken = token;
                }
                else
                {
                    throw new InvalidOperationException($"Missing configuration token {CONFIG_DISCORD_TOKEN}");
                }
                
                var discord_service_success = services.GetService<IDiscordProvider>(out var provider);
                if (discord_service_success)
                {
                    Backend = provider;
                    Backend.Setup(DiscordToken);
                }
            });
        }
        
        private void OnServiceUnload(string service_name)
        {
            if (service_name == (Backend?.Name ?? string.Empty))
            {
                DisposeOldBackend();
            }
        }
        
        private void OnServiceLoad(IService service)
        {
            if (service is IDiscordProvider idp)
            {
                if (!(Backend is null) && (service.Name != Backend.Name))
                {
                    DisposeOldBackend();
                }
                
                Backend = idp;
                Backend.Setup(DiscordToken);
            }
        }
        
        private void DisposeOldBackend()
        {
            Backend?.Disconnect();
            Backend = null;
            GC.Collect(); // to help AssemblyLoadContext unload the module.
        }
    }
}
