
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Services.Providers;
    
    public class DiscordConfig
    {
        public ProviderConfig Provider { get; set; }
        
        public DiscordConfig()
        {
            Provider = new ProviderConfig();
        }
    }
}
