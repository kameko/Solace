
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
        
        Task Send(ulong channel, string message);
        Task Connect();
        Task Disconnect();
        Task SetAvatar(string url);
        Task SetUsername(string name);
        Task SetNickname(ulong guild, string name);
        Task SetStatus(string status);
        Task PingLoop(CancellationToken token, int timeout, int tries);
        
        // TODO: ping, reconnect
        // TODO: sending attachments
        // TODO: query guilds/channels for messages
    }
}
