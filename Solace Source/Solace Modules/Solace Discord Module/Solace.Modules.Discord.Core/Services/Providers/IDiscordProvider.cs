
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IDiscordProvider : IProvider
    {
        event Func<SolaceDiscordMessage, Task> OnReceiveMessage;
        
        Task Setup(string token);
        Task Connect();
        void Disconnect();
        
        // TODO: sending messages
        // TODO: query guilds/channels for messages
    }
}
