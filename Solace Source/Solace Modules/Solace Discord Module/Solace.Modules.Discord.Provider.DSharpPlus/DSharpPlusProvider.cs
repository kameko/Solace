
namespace Solace.Modules.Discord.Provider.DSharpPlus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Solace.Core;
    using Solace.Core.Services;
    using Core;
    using Core.Services.Providers;
    using global::DSharpPlus;
    using global::DSharpPlus.EventArgs;
    using global::DSharpPlus.Entities;
    
    public class DSharpPlusProvider : BaseDiscordProvider
    {
        private DiscordClient Client { get; set; }
        private ProviderConfig Config { get; set; }
        private bool Configured => Config is null || Client is null;
        
        public DSharpPlusProvider() : base()
        {
            MaxQueryLimit = 100;
            Client        = null!;
            Config        = null!;
        }
        
        // --- Setup --- //
        
        public override Task Setup(IConfiguration config, ServiceProvider services)
        {
            if (config is ProviderConfig pc)
            {
                return Task.Run(() => Setup(pc));
            }
            else
            {
                throw new ArgumentException($"config is not of type {nameof(ProviderConfig)}");
            }
        }
        
        public Task Setup(ProviderConfig config)
        {
            Config = config;
            
            var dc = new DiscordConfiguration
            {
                Token                 = config.ConnectionToken,
                TokenType             = TokenType.Bot,
                AutoReconnect         = true,
                LogLevel              = ConvertLogLevel(config.LogLevel),
                UseInternalLogHandler = false,
            };
            
            Client = new DiscordClient(dc);
            
            Client.DebugLogger.LogMessageReceived += ClientOnLogMessageReceived;
            Client.Ready                          += ClientOnReady;
            Client.Resumed                        += ClientOnResume;
            Client.ClientErrored                  += ClientOnClientError;
            Client.WebhooksUpdated                += ClientOnWebhooksUpdated;
            Client.VoiceStateUpdated              += ClientOnVoiceStateUpdated;
            Client.VoiceServerUpdated             += ClientOnVoiceServerUpdated;
            Client.UserUpdated                    += ClientOnUserUpdated;
            Client.UserSettingsUpdated            += ClientOnUserSettingsUpdated;
            Client.PresenceUpdated                += ClientOnPresenceUpdated;
            Client.TypingStarted                  += ClientOnTypingStarted;
            Client.SocketOpened                   += ClientOnSocketOpened;
            Client.SocketErrored                  += ClientOnSocketError;
            Client.SocketClosed                   += ClientOnSocketClosed;
            Client.Heartbeated                    += ClientOnHeartbeat;
            Client.MessageCreated                 += ClientOnMessageCreated;
            Client.MessageUpdated                 += ClientOnMessageUpdated;
            Client.MessageAcknowledged            += ClientOnMessageAcknowledged;
            Client.MessageDeleted                 += ClientOnMessageDeleted;
            Client.MessagesBulkDeleted            += ClientOnMessagesBulkDeleted;
            Client.MessageReactionsCleared        += ClientOnMessageReactionsCleared;
            Client.MessageReactionRemoved         += ClientOnMessageReactionRemoved;
            Client.MessageReactionAdded           += ClientOnMessageReactionAdded;
            Client.DmChannelCreated               += ClientOnDmChannelCreated;
            Client.DmChannelDeleted               += ClientOnDmChannelDeleted;
            Client.ChannelCreated                 += ClientOnChannelCreated;
            Client.ChannelUpdated                 += ClientOnChannelUpdated;
            Client.ChannelPinsUpdated             += ClientOnChannelPinsUpdated;
            Client.ChannelDeleted                 += ClientOnChannelDeleted;
            Client.GuildCreated                   += ClientOnGuildCreated;
            Client.GuildUpdated                   += ClientOnGuildUpdated;
            Client.GuildDeleted                   += ClientOnGuildDeleted;
            Client.GuildAvailable                 += ClientOnGuildAvailable;
            Client.GuildUnavailable               += ClientOnGuildUnavailable;
            Client.GuildDownloadCompleted         += ClientOnGuildDownloadComplete;
            Client.GuildRoleCreated               += ClientOnGuildRoleCreated;
            Client.GuildRoleUpdated               += ClientOnGuildRoleUpdated;
            Client.GuildRoleDeleted               += ClientOnGuildRoleDeleted;
            Client.GuildMemberAdded               += ClientOnGuildMemberAdded;
            Client.GuildMemberUpdated             += ClientOnGuildMemberUpdated;
            Client.GuildMemberRemoved             += ClientOnGuildMemberRemoved;
            Client.GuildMembersChunked            += ClientOnGuildMembersChunked;
            Client.GuildBanAdded                  += ClientOnGuildBanAdded;
            Client.GuildBanRemoved                += ClientOnGuildBanRemoved;
            Client.GuildEmojisUpdated             += ClientOnGuildEmojisUpdated;
            Client.GuildIntegrationsUpdated       += ClientOnGuildIntegrationsUpdated;
            Client.UnknownEvent                   += ClientOnUnknownEvent;
            
            return Task.CompletedTask;
        }
        
        // --- Public Methods --- //
        
        public override async Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id, ulong starting_message_id)
        {
            var dcqt = new DiscordChannelQueryToken(this, channel_id, starting_message_id);
            var success = await dcqt.Setup();
            if (success)
            {
                return dcqt;
            }
            return null;
        }
        
        public override async Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id)
        {
            var current = await QueryLatest(channel_id);
            if (current is null)
            {
                return null;
            }
            
            return await QueryChannel(channel_id, current.MessageId);
        }
        
        public override async Task<SolaceDiscordMessage?> GetMessage(ulong channel_id, ulong message_id)
        {
            CheckConfigured();
            try
            {
                var channel = await Client.GetChannelAsync(channel_id);
                var rawmsg  = await channel.GetMessageAsync(message_id);
                var message = await ConvertMessage(rawmsg);
                return message;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return null;
            }
        }
        
        public override async Task<SolaceDiscordMessage?> QueryLatest(ulong channel_id)
        {
            CheckConfigured();
            try
            {
                var channel = await Client.GetChannelAsync(channel_id);
                var rawmsg  = await channel.GetMessageAsync(channel.LastMessageId);
                var message = await ConvertMessage(rawmsg);
                return message;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return null;
            }
        }
        
        public override async Task<IEnumerable<SolaceDiscordMessage>?> QueryLatest(ulong channel_id, int limit)
        {
            CheckConfigured();
            try
            {
                var channel = await Client.GetChannelAsync(channel_id);
                var latest = await channel.GetMessagesAsync(limit);
                return latest.ToList() as IEnumerable<SolaceDiscordMessage>;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return null;
            }
        }
        
        public override async Task<IEnumerable<SolaceDiscordMessage>?> QueryBefore(ulong channel_id, ulong before_message_id, int limit)
        {
            CheckConfigured();
            try
            {
                var channel = await Client.GetChannelAsync(channel_id);
                var messages = await channel.GetMessagesBeforeAsync(before_message_id, limit);
                return messages.ToList() as IEnumerable<SolaceDiscordMessage>;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return null;
            }
        }
        
        public override async Task<IEnumerable<SolaceDiscordMessage>?> QueryAfter(ulong channel_id, ulong before_message_id, int limit)
        {
            CheckConfigured();
            try
            {
                var channel = await Client.GetChannelAsync(channel_id);
                var messages = await channel.GetMessagesAfterAsync(before_message_id, limit);
                return messages.ToList() as IEnumerable<SolaceDiscordMessage>;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return null;
            }
        }
        
        public override async Task<bool> Send(ulong channel, string message)
        {
            CheckConfigured();
            try
            {
                var dc = await Client.GetChannelAsync(channel);
                await Client.SendMessageAsync(dc, message);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return false;
            }
        }
        
        // TODO: resource gathering, might need a new object to represent the resource
        // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus/Entities/DiscordEmbedBuilder.cs 
        public override async Task<bool> Send(ulong channel_id, string message, Stream resource, string filename)
        {
            CheckConfigured();
            var channel = await Client.GetChannelAsync(channel_id);
            // await channel.SendFileAsync()
            throw new NotImplementedException();
        }
        
        public override async Task<bool> Send(ulong channel_id, Stream resource, string filename)
        {
            return await Send(channel_id, string.Empty, resource, filename);
        }
        
        public override async Task<bool> Connect()
        {
            CheckConfigured();
            if (Connected)
            {
                return false;
            }
            
            try
            {
                await Client.ConnectAsync();
                Connected = true;
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return false;
            }
        }
        
        public override async Task<bool> Disconnect()
        {
            CheckConfigured();
            if (!Connected)
            {
                return false;
            }
            
            try
            {
                Connected = false;
                await Client.DisconnectAsync();
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return false;
            }
        }
        
        public override async Task<bool> SetAvatar(Stream file_stream)
        {
            CheckConfigured();
            try
            {
                await Client.UpdateCurrentUserAsync(null, file_stream);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return false;
            }
        }
        
        public override async Task<bool> SetUsername(string name)
        {
            CheckConfigured();
            try
            {
                await Client.UpdateCurrentUserAsync(name);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return false;
            }
        }
        
        public override async Task<bool> SetNickname(ulong guild, string name)
        {
            CheckConfigured();
            try
            {
                var user = await Client.GetGuildAsync(guild);
                await user.CurrentMember.ModifyAsync(m =>
                {
                    m.Nickname = name;
                });
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return false;
            }
        }
        
        public override async Task<bool> SetStatus(string status)
        {
            return await SetStatus(status, SolaceDiscordActivity.Playing);
        }
        
        public override async Task<bool> SetStatus(string status, SolaceDiscordActivity activity_kind)
        {
            CheckConfigured();
            try
            {
                ActivityType at = activity_kind switch
                {
                    SolaceDiscordActivity.Playing     => ActivityType.Playing,
                    SolaceDiscordActivity.Watching    => ActivityType.Watching,
                    SolaceDiscordActivity.ListeningTo => ActivityType.ListeningTo,
                    _                                 => ActivityType.Playing
                };
                var activity = new DiscordActivity(status, at);
                await Client.UpdateStatusAsync(activity);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return false;
            }
        }
        
        public override Task PingLoop(CancellationToken token, int timeout, int tries)
        {
            // TODO: create a task to constantly ping and
            // try to connect if it doesn't get a response.
            // needs to be configurable, but should default
            // to around a ping every two seconds. If it doesn't
            // get 3 pings in a row, disconnect and reconnect in
            // a loop.
            // Might not actually be needed, D#+ apparently handles this.
            
            throw new NotImplementedException();
        }
        
        // --- Client Event Handlers --- //
        
        private void ClientOnLogMessageReceived(object? o, DebugLogMessageEventArgs e)
        {
            if (!Config.DebugLog)
            {
                return;
            }
            
            if (e.Exception is null)
            {
                switch (e.Level)
                {
                    case LogLevel.Info:
                        Log.Info(e.Message);
                        break;
                    case LogLevel.Warning:
                        Log.Warning(e.Message);
                        break;
                    case LogLevel.Error:
                        Log.Error(e.Message);
                        break;
                    case LogLevel.Critical:
                        Log.Fatal(e.Message);
                        break;
                    case LogLevel.Debug:
                        Log.Debug(e.Message);
                        break;
                    default:
                        Log.Debug($"UNHANDLED LOG LEVEL \"{e.Level}\" Message: {e.Message}");
                        break;
                }
            }
            else
            {
                switch (e.Level)
                {
                    case LogLevel.Info:
                        Log.Info(e.Exception, e.Message);
                        break;
                    case LogLevel.Warning:
                        Log.Warning(e.Exception, e.Message);
                        break;
                    case LogLevel.Error:
                        Log.Error(e.Exception, e.Message);
                        break;
                    case LogLevel.Critical:
                        Log.Fatal(e.Exception, e.Message);
                        break;
                    case LogLevel.Debug:
                        Log.Debug(e.Exception, e.Message);
                        break;
                    default:
                        Log.Debug(e.Exception, $"UNHANDLED LOG LEVEL \"{e.Level}\" Message: {e.Message}");
                        break;
                }
            }
        }
        
        private async Task ClientOnReady(ReadyEventArgs e)
        {
            Log.Info("Client ready");
            Ready = true;
            await RaiseOnReady();
        }
        
        private Task ClientOnResume(ReadyEventArgs e)
        {
            Log.Info("Client resumed");
            return Task.CompletedTask;
        }
        
        private Task ClientOnClientError(ClientErrorEventArgs e)
        {
            Log.Error(e.Exception, $"Client error: {e.EventName}");
            return Task.CompletedTask;
        }
        
        private Task ClientOnWebhooksUpdated(WebhooksUpdateEventArgs e)
        {
            Log.Info($"Client webhooks updated for \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})");
            return Task.CompletedTask;
        }
        
        private Task ClientOnVoiceStateUpdated(VoiceStateUpdateEventArgs e)
        {
            Log.Info($"Client voice state changed for \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})");
            return Task.CompletedTask;
        }
        
        private Task ClientOnVoiceServerUpdated(VoiceServerUpdateEventArgs e)
        {
            Log.Info($"Client voice server changed for \"{e.Guild.Name}\" ({e.Guild.Id}) to {e.Endpoint}");
            return Task.CompletedTask;
        }
        
        private Task ClientOnUserUpdated(UserUpdateEventArgs e)
        {
            // TODO: find a nice way to log this, find the difference in Before and After.
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnUserSettingsUpdated(UserSettingsUpdateEventArgs e)
        {
            // TODO: figure out what this means and if/how to log it.
            return Task.CompletedTask;
        }
        
        private Task ClientOnPresenceUpdated(PresenceUpdateEventArgs e)
        {
            // TODO: this is a lot to log, find a nice way to make it coherent
            return Task.CompletedTask;
        }
        
        private Task ClientOnTypingStarted(TypingStartEventArgs e)
        {
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnSocketOpened()
        {
            Log.Debug("Client socket opened");
            return Task.CompletedTask;
        }
        
        private Task ClientOnSocketError(SocketErrorEventArgs e)
        {
            Log.Error(e.Exception, "Client socket error");
            return Task.CompletedTask;
        }
        
        private Task ClientOnSocketClosed(SocketCloseEventArgs e)
        {
            Log.Info($"Client socket closed. Reason: {e.CloseMessage} ({e.CloseCode})");
            return Task.CompletedTask;
        }
        
        private Task ClientOnHeartbeat(HeartbeatEventArgs e)
        {
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private async Task ClientOnMessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsCurrent)
            {
                return;
            }
            
            if (e.Channel.Type.HasFlag(ChannelType.Text))
            {
                try
                {
                    await ProcessOnTextMessageCreated(e);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, string.Empty);
                }
            }
            else
            {
                Log.Warning($"Unhandled Discord message type: {e.Channel.Type}");
            }
        }
        
        private Task ClientOnMessageUpdated(MessageUpdateEventArgs e)
        {
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnMessageAcknowledged(MessageAcknowledgeEventArgs e)
        {
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnMessageDeleted(MessageDeleteEventArgs e)
        {
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnMessagesBulkDeleted(MessageBulkDeleteEventArgs e)
        {
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnMessageReactionsCleared(MessageReactionsClearEventArgs e)
        {
            Log.Info(
                $"Message reactions cleared for message {e.Message.Id} \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}). "
              + (string.IsNullOrEmpty(e.Message.Content) ? string.Empty : $"Message Content: {e.Message.Content}")
            );
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnMessageReactionRemoved(MessageReactionRemoveEventArgs e)
        {
            Log.Info(
                $"Message reaction {e.Emoji.GetDiscordName()} removed for message "
              + $"{e.Message.Id} \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}). "
              + (string.IsNullOrEmpty(e.Message.Content) ? string.Empty : $"Message Content: {e.Message.Content}")
            );
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnMessageReactionAdded(MessageReactionAddEventArgs e)
        {
            Log.Info(
                $"Message reaction {e.Emoji.GetDiscordName()} added for message "
              + $"{e.Message.Id} \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}). "
              + (string.IsNullOrEmpty(e.Message.Content) ? string.Empty : $"Message Content: {e.Message.Content}")
            );
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnDmChannelCreated(DmChannelCreateEventArgs e)
        {
            Log.Info($"DM created with {ConcatMembers(e.Channel.Users)}");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnDmChannelDeleted(DmChannelDeleteEventArgs e)
        {
            Log.Info($"DM deleted with {ConcatMembers(e.Channel.Users)}");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnChannelCreated(ChannelCreateEventArgs e)
        {
            Log.Info($"Channel created \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnChannelUpdated(ChannelUpdateEventArgs e)
        {
            // TODO: more logging coherency, difference between Before and After
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnChannelPinsUpdated(ChannelPinsUpdateEventArgs e)
        {
            Log.Info($"Pins updated in \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})");
            // TODO: maybe get the new pinned message? not sure how.
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnChannelDeleted(ChannelDeleteEventArgs e)
        {
            Log.Info($"Channel deleted \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildCreated(GuildCreateEventArgs e)
        {
            Log.Info($"Guild created \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildUpdated(GuildUpdateEventArgs e)
        {
            // TODO: yeah, big Before and After difference checker...
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildDeleted(GuildDeleteEventArgs e)
        {
            Log.Info($"Guild deleted \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildAvailable(GuildCreateEventArgs e)
        {
            Log.Info($"Guild available \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: event for this? or same event as Created
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildUnavailable(GuildDeleteEventArgs e)
        {
            Log.Info($"Guild unavailable \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: event for this? or same event as Deleted
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildDownloadComplete(GuildDownloadCompletedEventArgs e)
        {
            // TODO: unsure
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildRoleCreated(GuildRoleCreateEventArgs e)
        {
            Log.Info($"Role \"{e.Role.Name}\" ({e.Role.Id}) created in \"{e.Guild.Name}\" ({e.Guild.Id})");
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildRoleUpdated(GuildRoleUpdateEventArgs e)
        {
            // TODO: check roles before and after, reuse for OnGuildMemberUpdated
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildRoleDeleted(GuildRoleDeleteEventArgs e)
        {
            Log.Info($"Role \"{e.Role.Name}\" ({e.Role.Id}) deleted in \"{e.Guild.Name}\" ({e.Guild.Id})");
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildMemberAdded(GuildMemberAddEventArgs e)
        {
            Log.Info($"User {e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id}) added to \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildMemberUpdated(GuildMemberUpdateEventArgs e)
        {
            // TODO: check roles before/after, and log if nicknamed changes
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildMemberRemoved(GuildMemberRemoveEventArgs e)
        {
            Log.Info($"User {e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id}) removed from \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildMembersChunked(GuildMembersChunkEventArgs e)
        {
            // TODO: unsure
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildBanAdded(GuildBanAddEventArgs e)
        {
            Log.Info($"User {e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id}) banned from \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildBanRemoved(GuildBanRemoveEventArgs e)
        {
            Log.Info($"Ban on user {e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id}) lifted from \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildEmojisUpdated(GuildEmojisUpdateEventArgs e)
        {
            // TODO: before/after
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildIntegrationsUpdated(GuildIntegrationsUpdateEventArgs e)
        {
            // TODO: unsure
            return Task.CompletedTask;
        }
        
        private Task ClientOnUnknownEvent(UnknownEventArgs e)
        {
            Log.Warning($"Client unhandled event: {e.EventName}. JSON: {e.Json}");
            return Task.CompletedTask;
        }
        
        // --- Private Methods --- //
        
        private void CheckConfigured()
        {
            if (!Configured)
            {
                throw new InvalidOperationException("Provider is not configured yet.");
            }
        }
        
        private async Task ProcessOnTextMessageCreated(MessageCreateEventArgs discord_message)
        {
            var message = await ConvertMessage(discord_message.Message);
            await RaiseOnReceiveMessage(message);
        }
        
        private string ConcatMembers(IEnumerable<DiscordMember> discord_users)
        {
            var users_sb = new StringBuilder(discord_users.Count() * 3);
            var index = 0;
            foreach (var user in discord_users)
            {
                if (!string.IsNullOrEmpty(user.Nickname) && user.Nickname != user.Username)
                {
                    users_sb.Append($"\"{user.Nickname}\"");
                }
                users_sb.Append($"{user.Username}#{user.Discriminator} ({user.Id})");
                if (discord_users.Count() > 1 && index < discord_users.Count() - 1)
                {
                    users_sb.Append(", ");
                }
                
                index++;
            }
            
            return users_sb.ToString();
        }
        
        private LogLevel ConvertLogLevel(Log.LogLevel sollevel)
        {
            switch (sollevel)
            {
                case Log.LogLevel.Info:    return LogLevel.Info;
                case Log.LogLevel.Warning: return LogLevel.Warning;
                case Log.LogLevel.Error:   return LogLevel.Error;
                case Log.LogLevel.Fatal:   return LogLevel.Critical;
                case Log.LogLevel.Debug:   return LogLevel.Debug;
                default:                   return LogLevel.Debug;
            }
        }
        
        private Log.LogLevel ConvertLogLevel(LogLevel provlevel)
        {
            switch (provlevel)
            {
                case LogLevel.Info:     return Log.LogLevel.Info;
                case LogLevel.Warning:  return Log.LogLevel.Warning;
                case LogLevel.Error:    return Log.LogLevel.Error;
                case LogLevel.Critical: return Log.LogLevel.Fatal;
                case LogLevel.Debug:    return Log.LogLevel.Debug;
                default:                return Log.LogLevel.Debug;
            }
        }
        
        private Task<SolaceDiscordMessage> ConvertMessage(DiscordMessage discord_message)
        {
            var discriminator = 0;
            var is_dm         = false;
            var nickname      = string.Empty;
            
            _ = int.TryParse(discord_message.Author.Discriminator, out discriminator);
            
            if (discord_message.Channel.Type.HasFlag(ChannelType.Private))
            {
                is_dm = true;
            }
            else
            {
                is_dm = false;
                if (discord_message.Author is DiscordMember dm)
                {
                    nickname = dm.Nickname;
                }
                if (string.IsNullOrEmpty(nickname))
                {
                    nickname = string.Empty;
                }
            }
            
            var message = new SolaceDiscordMessage()
            {
                Created   = discord_message.CreationTimestamp,
                Sender    = new SolaceDiscordUser()
                {
                    Username      = discord_message.Author.Username,
                    Discriminator = discriminator,
                    Id            = discord_message.Author.Id,
                    IsBot         = discord_message.Author.IsBot,
                    AvatarHash    = discord_message.Author.AvatarHash,
                },
                IsDM      = is_dm,
                Nickname  = is_dm ? string.Empty : nickname,
                GuildName = is_dm ? string.Empty : discord_message.Channel.Guild.Name,
                GuildId   = is_dm ? 0L : discord_message.Channel.Guild.Id,
                Channel   = new SolaceDiscordChannel()
                {
                    Name      = discord_message.Channel.Name,
                    Id        = discord_message.Channel.Id,
                    GuildName = is_dm ? string.Empty : discord_message.Channel.Guild.Name,
                    GuildId   = is_dm ? 0L : discord_message.Channel.GuildId,
                },
                MessageId = discord_message.Id,
                Message   = discord_message.Content,
            };
            
            message.Sender.TrySetUrl(discord_message.Author.AvatarUrl);
            
            foreach (var user in discord_message.MentionedUsers)
            {
                _ = int.TryParse(discord_message.Author.Discriminator, out int user_discriminator);
                
                var nuser = new SolaceDiscordUser()
                {
                    Username      = user.Username,
                    Discriminator = user_discriminator,
                    Id            = user.Id,
                    IsBot         = user.IsBot,
                    AvatarHash    = user.AvatarHash,
                };
                
                nuser.TrySetUrl(user.AvatarUrl);
                
                message.MentionedUsers.Add(nuser);
            }
            
            foreach (var channel in discord_message.MentionedChannels)
            {
                var nchannel = new SolaceDiscordChannel()
                {
                    Name    = channel.Name,
                    Id      = channel.Id,
                    GuildId = channel.GuildId,
                };
                
                message.MentionedChannels.Add(nchannel);
            }
            
            foreach (var role in discord_message.MentionedRoles)
            {
                var nrole = new SolaceDiscordRole()
                {
                    Name = role.Name,
                    Id   = role.Id,
                };
                
                message.MentionedRoles.Add(nrole);
            }
            
            foreach (var reaction in discord_message.Reactions)
            {
                var emoji = new SolaceDiscordEmoji()
                {
                    Name = reaction.Emoji.Name,
                    Id   = reaction.Emoji.Id,
                };
                
                emoji.TrySetUrl(reaction.Emoji.Url);
                
                message.Reactions.Add(emoji);
            }
            
            foreach (var attachment in discord_message.Attachments)
            {
                var token = new AttachmentToken()
                {
                    FileName = attachment.FileName,
                    Id       = attachment.Id,
                    FileSize = attachment.FileSize,
                };
                
                token.TrySetUrl(attachment.Url);
                token.TrySetProxyUrl(attachment.ProxyUrl);
                
                message.Attachments.Add(token);
            }
            
            return Task.FromResult(message);
        }
        
        public override async ValueTask DisposeAsync()
        {
            await Disconnect();
        }
    }
}
