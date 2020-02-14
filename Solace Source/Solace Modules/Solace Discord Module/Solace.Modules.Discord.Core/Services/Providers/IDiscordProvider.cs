
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
        int MaxQueryLimit { get; }
        event Func<bool, Task> OnReady;
        event Func<DifferenceTokens.VoiceStateDifference, Task> OnVoiceStateChange;
        event Func<DifferenceTokens.UserDifference, Task> OnUserUpdated;
        event Func<SolaceDiscordUser, Task> OnUserSettingsUpdated;
        event Func<DifferenceTokens.PresenceUpdatedDifference, Task> OnPresenceUpdated;
        event Func<SolaceDiscordUser, SolaceDiscordChannel, Task> OnUserTyping;
        event Func<SolaceDiscordHeartbeat, Task> OnHeartbeat;
        event Func<SolaceDiscordMessage, Task> OnReceiveMessage;
        event Func<DifferenceTokens.MessageDifference, Task> OnMessageUpdated;
        event Func<SolaceDiscordMessage, Task> OnMessageAcknowledged;
        event Func<SolaceDiscordMessage, Task> OnMessageDeleted;
        event Func<SolaceDiscordChannel, IEnumerable<SolaceDiscordMessage>, Task> OnBulkMessageDeletion;
        event Func<SolaceDiscordMessage, SolaceDiscordUser, SolaceDiscordEmoji, Task> OnReactionAdded;
        event Func<SolaceDiscordMessage, SolaceDiscordEmoji, Task> OnReactionRemoved;
        event Func<SolaceDiscordMessage, Task> OnAllReactionsRemoved;
        event Func<SolaceDiscordChannel, IEnumerable<SolaceDiscordUser>, Task> OnDmCreated;
        event Func<SolaceDiscordChannel, IEnumerable<SolaceDiscordUser>, Task> OnDmDeleted;
        event Func<SolaceDiscordChannel, Task> OnChannelCreated;
        event Func<DifferenceTokens.ChannelDifference, Task> OnChannelUpdated;
        event Func<SolaceDiscordChannel, DateTimeOffset?, Task> OnChannelPinsUpdated;
        event Func<SolaceDiscordChannel, Task> OnChannelDeleted;
        event Func<SolaceDiscordGuild, Task> OnGuildCreated;
        event Func<DifferenceTokens.GuildDifference, Task> OnGuildUpdated;
        event Func<SolaceDiscordGuild, Task> OnGuildDeleted;
        event Func<SolaceDiscordGuild, Task> OnGuildAvailable;
        event Func<SolaceDiscordGuild, Task> OnGuildUnavailable;
        event Func<SolaceDiscordGuild, SolaceDiscordUser, Task> OnGuildUserAdded;
        event Func<DifferenceTokens.GuildUserDifference, Task> OnGuildUserUpdated;
        event Func<SolaceDiscordGuild, SolaceDiscordUser, Task> OnGuildUserRemoved;
        event Func<SolaceDiscordGuild, SolaceDiscordUser, Task> OnGuildUserBanned;
        event Func<SolaceDiscordGuild, SolaceDiscordUser, Task> OnGuildUserUnbanned;
        
        Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id, ulong starting_message_id);
        Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id);
        Task<SolaceDiscordMessage?> GetMessage(ulong channel_id, ulong message_id);
        Task<SolaceDiscordMessage?> QueryLatest(ulong channel_id);
        Task<IEnumerable<SolaceDiscordMessage>?> QueryLatest(ulong channel_id, int limit);
        Task<IEnumerable<SolaceDiscordMessage>?> QueryBefore(ulong channel_id, ulong before_message_id, int limit);
        Task<IEnumerable<SolaceDiscordMessage>?> QueryAfter(ulong channel_id, ulong before_message_id, int limit);
        Task StartTyping(ulong channel_id);
        Task<bool> Send(ulong channel, string message);
        Task<bool> Send(ulong channel_id, string message, Stream resource, string filename);
        Task<bool> Send(ulong channel_id, Stream resource, string filename);
        Task<bool> Connect();
        Task<bool> Disconnect();
        Task<SolaceDiscordGuild?> GetGuild(ulong guild_id);
        Task<IEnumerable<SolaceDiscordGuild>> GetGuilds();
        Task<SolaceDiscordChannel?> GetChannel(ulong channel_id);
        Task<IEnumerable<SolaceDiscordChannel>> GetChannels(ulong guild_id);
        Task<SolaceDiscordUser?> GetUser(ulong user_id);
        Task<IEnumerable<SolaceDiscordUser>> GetUsers(ulong guild_id);
        Task<bool> SetAvatar(Stream file_stream);
        Task<bool> SetUsername(string name);
        Task<bool> SetNickname(ulong guild, string name);
        Task<bool> SetStatus(string status);
        Task<bool> SetStatus(string status, SolaceDiscordActivity activity_kind);
        Task PingLoop(CancellationToken token, int timeout, int tries);
    }
}
