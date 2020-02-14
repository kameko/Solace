
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
        public event Func<SolaceDiscordMessage, Task> OnReceiveMessage;
        public event Func<DifferenceTokens.VoiceStateDifference, Task> OnVoiceStateChange;
        public event Func<DifferenceTokens.UserUpdatedDifference, Task> OnUserUpdated;
        public event Func<SolaceDiscordUser, Task> OnUserSettingsUpdated;
        public event Func<DifferenceTokens.PresenceUpdatedDifference, Task> OnPresenceUpdated;
        public event Func<SolaceDiscordUser, SolaceDiscordChannel, Task> OnUserTyping;
        public event Func<SolaceDiscordHeartbeat, Task> OnHeartbeat;
        
        public BaseDiscordProvider() : base()
        {
            OnReady               = delegate { return Task.CompletedTask; };
            OnReceiveMessage      = delegate { return Task.CompletedTask; };
            OnVoiceStateChange    = delegate { return Task.CompletedTask; };
            OnUserUpdated         = delegate { return Task.CompletedTask; };
            OnUserSettingsUpdated = delegate { return Task.CompletedTask; };
            OnPresenceUpdated     = delegate { return Task.CompletedTask; };
            OnUserTyping          = delegate { return Task.CompletedTask; };
            
            OnHeartbeat           = delegate { return Task.CompletedTask; };
        }
        
        protected async Task RaiseOnReceiveMessage(SolaceDiscordMessage message)
        {
            await OnReceiveMessage.Invoke(message);
        }
        
        protected async Task RaiseOnReady(bool resuming)
        {
            await OnReady.Invoke(resuming);
        }
        
        protected async Task RaiseOnReceiveMessage(DifferenceTokens.VoiceStateDifference difference)
        {
            await OnVoiceStateChange.Invoke(difference);
        }
        
        protected async Task RaiseOnUserUpdated(DifferenceTokens.UserUpdatedDifference difference)
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
