
namespace Solace.Core.Services.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IChatService : ICommunicationService
    {
        event Func<CommunicationMessage, Task> OnReceive;
        
        Task<bool> Send(CommunicationMessage message);
        Task<bool> Reconnect();
    }
}
