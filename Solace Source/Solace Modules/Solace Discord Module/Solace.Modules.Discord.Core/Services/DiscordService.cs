
namespace Solace.Modules.Discord.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core;
    using Solace.Core.Services;
    using Solace.Core.Services.Communication;
    
    // TODO: get the DSharpPlusProvider service
    
    public class DiscordService : BaseChatService
    {
        public DiscordService() : base()
        {
            
        }
        
        public override Task Setup(IConfiguration config, ServiceProvider services)
        {
            throw new NotImplementedException();
        }
    }
}
