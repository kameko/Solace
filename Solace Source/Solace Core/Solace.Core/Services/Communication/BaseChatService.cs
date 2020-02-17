
namespace Solace.Core.Services.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public abstract class BaseChatService : BaseCommunicationService, IChatService
    {
        public virtual event Func<CommunicationMessage, Task> OnReceive;
        
        public BaseChatService() : base()
        {
            OnReceive = delegate { return Task.CompletedTask; };
        }
        
        public virtual Task<bool> Send(CommunicationMessage message)
        {
            return Task.FromResult<bool>(false);
        }
        
        public virtual Task<bool> Reconnect()
        {
            return Task.FromResult<bool>(false);
        }
    }
}
