
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public abstract class BaseDiscordProvider : BaseProvider, IDiscordProvider
    {
        public bool Connected { get; protected set; }
        public event Func<SolaceDiscordMessage, Task> OnReceiveMessage;
        
        public BaseDiscordProvider() : base()
        {
            OnReceiveMessage = delegate { return Task.CompletedTask; };
        }
        
        protected async Task RaiseOnReceiveMessage(SolaceDiscordMessage message)
        {
            await OnReceiveMessage.Invoke(message);
        }
        
        public virtual Task<SolaceDiscordMessage> QueryOne(ulong channel_id)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> Send(ulong channel, string message)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> Connect()
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> Disconnect()
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> SetAvatar(string url)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> SetUsername(string name)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> SetNickname(ulong guild, string name)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> SetStatus(string status)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task PingLoop(CancellationToken token, int timeout, int tries)
        {
            throw new NotImplementedException();
        }
    }
}
