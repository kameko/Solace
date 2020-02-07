
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public abstract class DiscordProvider : BaseProvider, IDiscordProvider
    {
        public event Func<DiscordMessage, Task> OnReceiveMessage;
        
        public DiscordProvider() : base()
        {
            OnReceiveMessage = delegate { return Task.CompletedTask; };
        }
        
        protected async Task RaiseOnReceiveMessage(DiscordMessage message)
        {
            await OnReceiveMessage.Invoke(message);
        }
    }
}
