
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    
    public abstract class BaseDiscordProvider : BaseProvider, IDiscordProvider
    {
        public bool Connected { get; protected set; }
        public int MaxQueryLimit { get; protected set; }
        public event Func<bool, Task> OnReady;
        public event Func<DifferenceTokens.VoiceStateDifference, Task> OnVoiceStateChange;
        public event Func<DifferenceTokens.UserDifference, Task> OnUserUpdated;
        public event Func<SolaceDiscordUser, Task> OnUserSettingsUpdated;
        public event Func<DifferenceTokens.PresenceUpdatedDifference, Task> OnPresenceUpdated;
        public event Func<SolaceDiscordUser, SolaceDiscordChannel, Task> OnUserTyping;
        public event Func<SolaceDiscordMessage, Task> OnReceiveMessage;
        public event Func<DifferenceTokens.MessageDifference, Task> OnMessageUpdated;
        public event Func<SolaceDiscordMessage, Task> OnMessageAcknowledged;
        public event Func<SolaceDiscordMessage, Task> OnMessageDeleted;
        public event Func<SolaceDiscordChannel, IEnumerable<SolaceDiscordMessage>, Task> OnBulkMessageDeletion;
        public event Func<SolaceDiscordMessage, SolaceDiscordUser, SolaceDiscordEmoji, Task> OnReactionAdded;
        public event Func<SolaceDiscordMessage, SolaceDiscordEmoji, Task> OnReactionRemoved;
        public event Func<SolaceDiscordMessage, Task> OnAllReactionsRemoved;
        public event Func<SolaceDiscordChannel, IEnumerable<SolaceDiscordUser>, Task> OnDmCreated;
        public event Func<SolaceDiscordChannel, IEnumerable<SolaceDiscordUser>, Task> OnDmDeleted;
        public event Func<SolaceDiscordChannel, Task> OnChannelCreated;
        public event Func<DifferenceTokens.ChannelDifference, Task> OnChannelUpdated;
        public event Func<SolaceDiscordChannel, DateTimeOffset?, Task> OnChannelPinsUpdated;
        public event Func<SolaceDiscordChannel, Task> OnChannelDeleted;
        public event Func<SolaceDiscordGuild, Task> OnGuildCreated;
        public event Func<DifferenceTokens.GuildDifference, Task> OnGuildUpdated;
        public event Func<SolaceDiscordGuild, Task> OnGuildDeleted;
        public event Func<SolaceDiscordGuild, Task> OnGuildAvailable;
        public event Func<SolaceDiscordGuild, Task> OnGuildUnavailable;
        public event Func<SolaceDiscordGuild, SolaceDiscordUser, Task> OnGuildUserAdded;
        public event Func<DifferenceTokens.GuildUserDifference, Task> OnGuildUserUpdated;
        public event Func<SolaceDiscordGuild, SolaceDiscordUser, Task> OnGuildUserRemoved;
        public event Func<SolaceDiscordHeartbeat, Task> OnHeartbeat;
        
        public BaseDiscordProvider() : base()
        {
            OnReady               = delegate { return Task.CompletedTask; };
            OnVoiceStateChange    = delegate { return Task.CompletedTask; };
            OnUserUpdated         = delegate { return Task.CompletedTask; };
            OnUserSettingsUpdated = delegate { return Task.CompletedTask; };
            OnPresenceUpdated     = delegate { return Task.CompletedTask; };
            OnUserTyping          = delegate { return Task.CompletedTask; };
            OnReceiveMessage      = delegate { return Task.CompletedTask; };
            OnMessageUpdated      = delegate { return Task.CompletedTask; };
            OnMessageAcknowledged = delegate { return Task.CompletedTask; };
            OnMessageDeleted      = delegate { return Task.CompletedTask; };
            OnBulkMessageDeletion = delegate { return Task.CompletedTask; };
            OnReactionAdded       = delegate { return Task.CompletedTask; };
            OnReactionRemoved     = delegate { return Task.CompletedTask; };
            OnAllReactionsRemoved = delegate { return Task.CompletedTask; };
            OnDmCreated           = delegate { return Task.CompletedTask; };
            OnDmDeleted           = delegate { return Task.CompletedTask; };
            OnChannelCreated      = delegate { return Task.CompletedTask; };
            OnChannelUpdated      = delegate { return Task.CompletedTask; };
            OnChannelDeleted      = delegate { return Task.CompletedTask; };
            OnChannelPinsUpdated  = delegate { return Task.CompletedTask; };
            OnGuildCreated        = delegate { return Task.CompletedTask; };
            OnGuildUpdated        = delegate { return Task.CompletedTask; };
            OnGuildDeleted        = delegate { return Task.CompletedTask; };
            OnGuildAvailable      = delegate { return Task.CompletedTask; };
            OnGuildUnavailable    = delegate { return Task.CompletedTask; };
            OnGuildUserAdded      = delegate { return Task.CompletedTask; };
            OnGuildUserUpdated    = delegate { return Task.CompletedTask; };
            OnGuildUserRemoved    = delegate { return Task.CompletedTask; };
            
            OnHeartbeat           = delegate { return Task.CompletedTask; };
        }
        
        protected async Task RaiseOnReady(bool resuming)
        {
            await OnReady.Invoke(resuming);
        }
        
        protected async Task RaiseOnReceiveMessage(DifferenceTokens.VoiceStateDifference difference)
        {
            await OnVoiceStateChange.Invoke(difference);
        }
        
        protected async Task RaiseOnUserUpdated(DifferenceTokens.UserDifference difference)
        {
            await OnUserUpdated.Invoke(difference);
        }
        
        protected async Task RaiseOnUserSettingsUpdated(SolaceDiscordUser user)
        {
            await OnUserSettingsUpdated.Invoke(user);
        }
        
        protected async Task RaiseOnPresenceUpdated(DifferenceTokens.PresenceUpdatedDifference difference)
        {
            await OnPresenceUpdated.Invoke(difference);
        }
        
        protected async Task RaiseOnUserTyping(SolaceDiscordUser user, SolaceDiscordChannel channel)
        {
            await OnUserTyping.Invoke(user, channel);
        }
        
        protected async Task RaiseOnReceiveMessage(SolaceDiscordMessage message)
        {
            await OnReceiveMessage.Invoke(message);
        }
        
        protected async Task RaiseOnMessageUpdated(DifferenceTokens.MessageDifference difference)
        {
            await OnMessageUpdated.Invoke(difference);
        }
        
        protected async Task RaiseOnMessageAcknowledged(SolaceDiscordMessage message)
        {
            await OnMessageAcknowledged.Invoke(message);
        }
        
        protected async Task RaiseOnMessageDeleted(SolaceDiscordMessage message)
        {
            await OnMessageDeleted.Invoke(message);
        }
        
        protected async Task RaiseOnBulkMessageDeletion(SolaceDiscordChannel channel, IEnumerable<SolaceDiscordMessage> messages)
        {
            await OnBulkMessageDeletion.Invoke(channel, messages);
        }
        
        protected async Task RaiseOnReactionAdded(SolaceDiscordMessage message, SolaceDiscordUser user, SolaceDiscordEmoji emoji)
        {
            await OnReactionAdded.Invoke(message, user, emoji);
        }
        
        protected async Task RaiseOnReactionRemoved(SolaceDiscordMessage message, SolaceDiscordEmoji emoji)
        {
            await OnReactionRemoved.Invoke(message, emoji);
        }
        
        protected async Task RaiseOnAllReactionsRemoved(SolaceDiscordMessage message)
        {
            await OnAllReactionsRemoved.Invoke(message);
        }
        
        protected async Task RaiseOnDmCreated(SolaceDiscordChannel channel, IEnumerable<SolaceDiscordUser> users)
        {
            await OnDmCreated.Invoke(channel, users);
        }
        
        protected async Task RaiseOnDmDeleted(SolaceDiscordChannel channel, IEnumerable<SolaceDiscordUser> users)
        {
            await OnDmDeleted.Invoke(channel, users);
        }
        
        protected async Task RaiseOnChannelCreated(SolaceDiscordChannel channel)
        {
            await OnChannelCreated.Invoke(channel);
        }
        
        protected async Task RaiseOnChannelUpdated(DifferenceTokens.ChannelDifference difference)
        {
            await OnChannelUpdated.Invoke(difference);
        }
        
        public async Task RaiseOnChannelPinsUpdated(SolaceDiscordChannel channel, DateTimeOffset? message_timestamp)
        {
            await OnChannelPinsUpdated.Invoke(channel, message_timestamp);
        }
        
        public async Task RaiseOnChannelDeleted(SolaceDiscordChannel channel)
        {
            await OnChannelDeleted.Invoke(channel);
        }
        
        public async Task RaiseOnGuildCreated(SolaceDiscordGuild guild)
        {
            await OnGuildCreated.Invoke(guild);
        }
        
        public async Task RaiseOnGuildUpdated(DifferenceTokens.GuildDifference difference)
        {
            await OnGuildUpdated.Invoke(difference);
        }
        
        public async Task RaiseOnGuildDeleted(SolaceDiscordGuild guild)
        {
            await OnGuildDeleted.Invoke(guild);
        }
        
        public async Task RaiseOnGuildAvailable(SolaceDiscordGuild guild)
        {
            await OnGuildAvailable.Invoke(guild);
        }
        
        public async Task RaiseOnGuildUnavailable(SolaceDiscordGuild guild)
        {
            await OnGuildUnavailable.Invoke(guild);
        }
        
        public async Task RaiseOnGuildUserAdded(SolaceDiscordGuild guild, SolaceDiscordUser user)
        {
            await OnGuildUserAdded.Invoke(guild, user);
        }
        
        public async Task RaiseOnGuildUserUpdated(DifferenceTokens.GuildUserDifference difference)
        {
            await OnGuildUserUpdated.Invoke(difference);
        }
        
        public async Task RaiseOnGuildUserRemoved(SolaceDiscordGuild guild, SolaceDiscordUser user)
        {
            await OnGuildUserRemoved.Invoke(guild, user);
        }
        
        // TODO: events
        
        
        protected async Task RaiseOnHeartbeat(SolaceDiscordHeartbeat heartbeat)
        {
            await OnHeartbeat.Invoke(heartbeat);
        }
        
        public virtual Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id, ulong starting_message_id)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<SolaceDiscordMessage?> GetMessage(ulong channel_id, ulong message_id)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<SolaceDiscordMessage?> QueryLatest(ulong channel_id)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<IEnumerable<SolaceDiscordMessage>?> QueryLatest(ulong channel_id, int limit)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<IEnumerable<SolaceDiscordMessage>?> QueryBefore(ulong channel_id, ulong before_message_id, int limit)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<IEnumerable<SolaceDiscordMessage>?> QueryAfter(ulong channel_id, ulong before_message_id, int limit)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> Send(ulong channel, string message)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> Send(ulong channel_id, string message, Stream resource, string filename)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> Send(ulong channel_id, Stream resource, string filename)
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
        
        public virtual Task<bool> SetAvatar(Stream file_stream)
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
        
        public virtual Task<bool> SetStatus(string status, SolaceDiscordActivity activity_kind)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task PingLoop(CancellationToken token, int timeout, int tries)
        {
            throw new NotImplementedException();
        }
    }
}
