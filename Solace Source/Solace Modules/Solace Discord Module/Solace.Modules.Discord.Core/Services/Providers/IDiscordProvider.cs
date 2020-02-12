
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    
    public interface IDiscordProvider : IProvider
    {
        bool Connected { get; }
        event Func<SolaceDiscordMessage, Task> OnReceiveMessage;
        
        Task<SolaceDiscordMessage?> QueryLatest(ulong channel_id);
        Task<IEnumerable<SolaceDiscordMessage>?> QueryLatest(ulong channel_id, int limit);
        Task<IEnumerable<SolaceDiscordMessage>?> QueryBefore(ulong channel_id, ulong before_message_id, int limit);
        Task<IEnumerable<SolaceDiscordMessage>?> QueryAfter(ulong channel_id, ulong before_message_id, int limit);
        Task<bool> Send(ulong channel, string message);
        Task<bool> Send(ulong channel_id, string message, Stream resource, string filename);
        Task<bool> Send(ulong channel_id, Stream resource, string filename);
        Task<bool> Connect();
        Task<bool> Disconnect();
        Task<bool> SetAvatar(Stream file_stream);
        Task<bool> SetUsername(string name);
        Task<bool> SetNickname(ulong guild, string name);
        Task<bool> SetStatus(string status);
        Task PingLoop(CancellationToken token, int timeout, int tries);
        
        // TODO: get all guilds and channels in a guild.
        // TODO: query guilds/channels for messages
        // will require returning a custom IEnumerable type to act as a stream
        // of messages that the system will query one-by-one, also a custom
        // IEnumerable that will query x-by-x to lessen the API load.
    }
}
