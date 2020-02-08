
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    
    public interface IDiscordProvider : IProvider
    {
        bool Connected { get; }
        event Func<SolaceDiscordMessage, Task> OnReceiveMessage;
        
        Task Setup(string token);
        Task Connect();
        Task Disconnect();
        Task PingLoop(CancellationToken token, int timeout, int tries);
        
        // TODO: ping, reconnect
        // TODO: sending messages
        // TODO: query guilds/channels for messages
    }
}
