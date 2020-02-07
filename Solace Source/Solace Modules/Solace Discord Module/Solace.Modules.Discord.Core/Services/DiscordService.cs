
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
        private IDiscordProvider? Backend { get; set; }
        
        public DiscordService() : base()
        {
            Backend = null;
        }
        
        private void OnServiceUnload(string service)
        {
            if (service == (Backend?.Name ?? string.Empty))
            {
                Backend = null;
            }
        }
        
        public override Task Setup(IConfiguration config, ServiceProvider services)
        {
            return Task.Run(() =>
            {
                services.OnServiceUnload += OnServiceUnload;
                
                var success = services.GetService<IDiscordProvider>(out var provider);
                if (success)
                {
                    Backend = provider;
                }
            });
        }
    }
}
