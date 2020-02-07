
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public abstract class DiscordProvider : BaseProvider, IDiscordProvider
    {
        public event Func<DiscordMessage, Task> OnReceive;
        
        public DiscordProvider() : base()
        {
            OnReceive = delegate { return Task.CompletedTask; };
        }
        
        protected async Task RaiseOnReceive(DiscordMessage message)
        {
            await OnReceive.Invoke(message);
        }
    }
}
