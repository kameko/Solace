
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
                "DISCORD.TOKEN",
            };
        }
        
        public override Task Setup(IConfiguration config, ServiceProvider services)
        {
            Services = services;
            return Task.Run(() =>
            {
                services.OnServiceUnload += OnServiceUnload;
                services.OnServiceLoad   += OnServiceLoad;
                
                var token = config.GetValue("DISCORD.TOKEN");
                if (!string.IsNullOrEmpty(token))
                {
                    DiscordToken = token;
                }
                else
                {
                    throw new InvalidOperationException("Missing configuration token DISCORD.TOKEN");
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
                Backend?.Disconnect();
                Backend = null;
                GC.Collect(); // to help AssemblyLoadContext unload the module.
            }
        }
        
        private void OnServiceLoad(IService service)
        {
            if (service is IDiscordProvider idp)
            {
                if (!(Backend is null) && (service.Name != Backend.Name))
                {
                    Backend.Disconnect();
                    Backend = null;
                    GC.Collect(); // to help AssemblyLoadContext unload the module.
                }
                
                Backend = idp;
                Backend.Setup(DiscordToken);
            }
        }
    }
}
