
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
        
        Task<bool> Send(ulong channel, string message);
        Task<bool> Connect();
        Task<bool> Disconnect();
        Task<bool> SetAvatar(string url);
        Task<bool> SetUsername(string name);
        Task<bool> SetNickname(ulong guild, string name);
        Task<bool> SetStatus(string status);
        Task PingLoop(CancellationToken token, int timeout, int tries);
        
        // TODO: ping, reconnect
        // TODO: sending attachments
        // TODO: query guilds/channels for messages
        // will require returning a custom IEnumerable type to act as a stream
        // of messages that the system will query one-by-one, also a custom
        // IEnumerable that will query x-by-x to lessen the API load.
        // Or instead of a custom object just use the yield statement.
    }
}
