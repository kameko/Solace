
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public abstract class BaseDiscordProvider : BaseProvider, IDiscordProvider
    {
        public event Func<DiscordMessage, Task> OnReceiveMessage;
        
        public BaseDiscordProvider() : base()
        {
            OnReceiveMessage = delegate { return Task.CompletedTask; };
        }
        
        protected async Task RaiseOnReceiveMessage(DiscordMessage message)
        {
            await OnReceiveMessage.Invoke(message);
        }
        
        public virtual Task Setup(string token)
        {
            return Task.CompletedTask;
        }
        
        public virtual Task Connect()
        {
            return Task.CompletedTask;
        }
        
        public virtual void Disconnect()
        {
            
        }
    }
}
