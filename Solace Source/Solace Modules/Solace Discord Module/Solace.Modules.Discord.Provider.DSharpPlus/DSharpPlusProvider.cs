
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
            Client.MessageReactionAdded           += ClientOnMessageReactionAdded;
            Client.MessageReactionRemoved         += ClientOnMessageReactionRemoved;
            Client.MessageReactionsCleared        += ClientOnMessageReactionsCleared;
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
            
            await RaiseOnReady(resuming: false);
        }
        
        private async Task ClientOnResume(ReadyEventArgs e)
        {
            Log.Info("Client resumed");
            Ready = true;
            
            await RaiseOnReady(resuming: true);
        }
        
        private Task ClientOnClientError(ClientErrorEventArgs e)
        {
            Log.Error(e.Exception, $"Client error: {e.EventName}");
            return Task.CompletedTask;
        }
        
        private Task ClientOnWebhooksUpdated(WebhooksUpdateEventArgs e)
        {
            Log.Verbose($"Client webhooks updated for \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})");
            return Task.CompletedTask;
        }
        
        private async Task ClientOnVoiceStateUpdated(VoiceStateUpdateEventArgs e)
        {
            var before = ConvertVoiceState(e.Before);
            var after = ConvertVoiceState(e.After);
            var diff = new DifferenceTokens.VoiceStateDifference(before, after);
            
            var source = $"\"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})";
            
            Log.Info($"Client voice state changed in {source}. Differences: {diff}");
            
            await RaiseOnReceiveMessage(diff);
        }
        
        private Task ClientOnVoiceServerUpdated(VoiceServerUpdateEventArgs e)
        {
            Log.Verbose($"Client voice server changed for \"{e.Guild.Name}\" ({e.Guild.Id}) to {e.Endpoint}");
            return Task.CompletedTask;
        }
        
        private async Task ClientOnUserUpdated(UserUpdateEventArgs e)
        {
            var before = ConvertUser(e.UserBefore);
            var after = ConvertUser(e.UserAfter);
            var diff = new DifferenceTokens.UserDifference(before, after);
            
            var user = $"{e.UserAfter.Username}#{e.UserAfter.Discriminator} ({e.UserAfter.Id})";
            
            Log.Info($"User {user} updated. Differences: {diff}");
            
            await RaiseOnUserUpdated(diff);
        }
        
        private async Task ClientOnUserSettingsUpdated(UserSettingsUpdateEventArgs e)
        {
            var user = ConvertUser(e.User);
            
            await RaiseOnUserSettingsUpdated(user);
        }
        
        private async Task ClientOnPresenceUpdated(PresenceUpdateEventArgs e)
        {
            var before_user = ConvertUser(e.UserBefore);
            var after_user = ConvertUser(e.UserAfter);
            var user_diff = new DifferenceTokens.UserDifference(before_user, after_user);
            
            // TODO: finish the presence class
            var user = ConvertUser(e.User);
            var presence_diff = new DifferenceTokens.PresenceDifference(user);
            
            var diff = new DifferenceTokens.PresenceUpdatedDifference(presence_diff, user_diff);
            
            await RaiseOnPresenceUpdated(diff);
        }
        
        private async Task ClientOnTypingStarted(TypingStartEventArgs e)
        {
            var user = ConvertUser(e.User);
            var channel = ConvertChannel(e.Channel);
            
            await RaiseOnUserTyping(user, channel);
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
        
        private async Task ClientOnHeartbeat(HeartbeatEventArgs e)
        {
            var heartbeat = new SolaceDiscordHeartbeat()
            {
                Timestamp         = e.Timestamp,
                Ping              = e.Ping,
                IntegrityChecksum = e.IntegrityChecksum
            };
            
            await RaiseOnHeartbeat(heartbeat);
        }
        
        private async Task ClientOnMessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsCurrent)
            {
                return;
            }
            
            if (e.Channel.Type.HasFlag(ChannelType.Text))
            {
                var is_dm = e.Channel.Type.HasFlag(ChannelType.Private);
                var source = is_dm ? $"DM {e.Channel.Id}" : $"\"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}";
                var user = $"{e.Author.Username}#{e.Author.Discriminator} ({e.Author.Id})";
                var log_msg = $"Message {e.Message.Id} received from {source} by user {user}";
                
                if (string.IsNullOrEmpty(e.Message.Content))
                {
                    log_msg += ". Content is empty";
                }
                else
                {
                    log_msg += $". Content: {SanitizeString(e.Message.Content)}. End Content";
                }
                
                var attach_no = e.Message.Attachments.Count();
                if (attach_no == 1)
                {
                    log_msg += ". One attachment included";
                }
                else if (attach_no > 1)
                {
                    log_msg += $". Number of attachments: {attach_no}";
                }
                
                Log.Info(log_msg);
                
                var message = await ConvertMessage(e.Message);
                await RaiseOnReceiveMessage(message);
            }
            else
            {
                Log.Warning($"Unhandled message type: {e.Channel.Type}");
            }
        }
        
        private async Task ClientOnMessageUpdated(MessageUpdateEventArgs e)
        {
            SolaceDiscordMessage? before = e.MessageBefore is null ? null : await ConvertMessage(e.MessageBefore);
            var after = await ConvertMessage(e.Message);
            var diff = new DifferenceTokens.MessageDifference(before, after);
            
            var is_dm = e.Channel.Type.HasFlag(ChannelType.Private);
            var source = is_dm ? $"DM {e.Channel.Id}" : $"\"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}";
            var log_msg = $"Message from user {e.Author.Username}#{e.Author.Discriminator} ({e.Author.Id}) in {source} updated";
            
            if (before is null)
            {
                log_msg += ". Previous message was not cached";
            }
            else if (string.IsNullOrEmpty(before.Message))
            {
                log_msg += ". Previous message content was empty";
            }
            else if (before.Message == after.Message)
            {
                log_msg += $". Content of previous message is the same";
            }
            else
            {
                log_msg += $". Previous Content: {SanitizeString(e.MessageBefore!.Content)}. End Previous Content";
            }
            
            if (string.IsNullOrEmpty(e.Message.Content))
            {
                log_msg += ". Current content is empty";
            }
            else
            {
                log_msg += $". Current Content: {SanitizeString(e.Message.Content)}. End Current Content";
            }
            
            var diff_str = diff.GetDifferenceString();
            if (string.IsNullOrEmpty(diff_str))
            {
                log_msg += $". No other differences found";
            }
            else
            {
                log_msg += $". Differences: {diff_str}";
            }
            
            Log.Info(log_msg);
            
            await RaiseOnMessageUpdated(diff);
        }
        
        private async Task ClientOnMessageAcknowledged(MessageAcknowledgeEventArgs e)
        {
            var message = await ConvertMessage(e.Message);
            await RaiseOnMessageAcknowledged(message);
        }
        
        private async Task ClientOnMessageDeleted(MessageDeleteEventArgs e)
        {
            var is_dm = e.Channel.Type.HasFlag(ChannelType.Private);
            var source = is_dm ? $"DM {e.Channel.Id}" : $"\"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}";
            var user = $"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id})";
            var log_msg = $"Message from user {user} in {source} deleted";
            
            if (string.IsNullOrEmpty(e.Message.Content))
            {
                log_msg += ". Content was empty";
            }
            else
            {
                log_msg += $". Content: {SanitizeString(e.Message.Content)}. End Content";
            }
            
            var attach_no = e.Message.Attachments.Count();
            if (attach_no == 1)
            {
                log_msg += $". Message contained an attachment";
            }
            else if (attach_no > 1)
            {
                log_msg += $". Message {attach_no} attachments";
            }
            
            Log.Info(log_msg);
            
            var message = await ConvertMessage(e.Message);
            await RaiseOnMessageDeleted(message);
        }
        
        private Task ClientOnMessagesBulkDeleted(MessageBulkDeleteEventArgs e)
        {
            var is_dm = e.Channel.Type.HasFlag(ChannelType.Private);
            var source = is_dm ? $"DM {e.Channel.Id}" : $"\"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}";
            Log.Info($"Bulk message deletion in {source}. Number of deleted messages: {e.Messages.Count()}");
            
            return Task.Run(async () =>
            {
                var channel = ConvertChannel(e.Channel);
                var messages = new List<SolaceDiscordMessage>(e.Messages.Count());
                
                foreach (var deleted in e.Messages)
                {
                    var message = await ConvertMessage(deleted);
                    messages.Add(message);
                }
                
                await RaiseOnBulkMessageDeletion(channel, messages);
            });
        }
        
        private async Task ClientOnMessageReactionAdded(MessageReactionAddEventArgs e)
        {
            Log.Info(
                $"Message reaction {e.Emoji.GetDiscordName()} added for message "
              + $"{e.Message.Id} \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})"
              + (string.IsNullOrEmpty(e.Message.Content) ? string.Empty : $". Content: {SanitizeString(e.Message.Content)}. End Content")
            );
            
            var message = await ConvertMessage(e.Message);
            var user    = ConvertUser(e.User);
            var emoji   = ConvertEmoji(e.Emoji);
            await RaiseOnReactionAdded(message, user, emoji);
        }
        
        private async Task ClientOnMessageReactionRemoved(MessageReactionRemoveEventArgs e)
        {
            Log.Info(
                $"Message reaction {e.Emoji.GetDiscordName()} removed for message "
              + $"{e.Message.Id} \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})"
              + (string.IsNullOrEmpty(e.Message.Content) ? string.Empty : $". Content: {SanitizeString(e.Message.Content)}. End Content")
            );
            
            var message = await ConvertMessage(e.Message);
            var emoji   = ConvertEmoji(e.Emoji);
            await RaiseOnReactionRemoved(message, emoji);
        }
        
        private async Task ClientOnMessageReactionsCleared(MessageReactionsClearEventArgs e)
        {
            Log.Info(
                $"Message reactions cleared for message {e.Message.Id} \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})"
              + (string.IsNullOrEmpty(e.Message.Content) ? string.Empty : $". Content: {SanitizeString(e.Message.Content)}. End Content")
            );
            
            var message = await ConvertMessage(e.Message);
            await RaiseOnAllReactionsRemoved(message);
        }
        
        private Task ClientOnDmChannelCreated(DmChannelCreateEventArgs e)
        {
            Log.Info($"DM {e.Channel.Id} with {ConcatMembers(e.Channel.Users)} created");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnDmChannelDeleted(DmChannelDeleteEventArgs e)
        {
            Log.Info($"DM {e.Channel.Id} with {ConcatMembers(e.Channel.Users)} deleted");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnChannelCreated(ChannelCreateEventArgs e)
        {
            Log.Info($"Channel {e.Channel.Name} ({e.Channel.Id}) created in \"{e.Guild.Name}\" ({e.Guild.Id})");
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
            Log.Info($"Channel {e.Channel.Name} ({e.Channel.Id}) deleted in \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildCreated(GuildCreateEventArgs e)
        {
            Log.Info($"Guild \"{e.Guild.Name}\" ({e.Guild.Id}) created");
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
            Log.Info($"Guild \"{e.Guild.Name}\" ({e.Guild.Id}) deleted");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildAvailable(GuildCreateEventArgs e)
        {
            Log.Info($"Guild \"{e.Guild.Name}\" ({e.Guild.Id}) is now available");
            // TODO: event for this? or same event as Created
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildUnavailable(GuildDeleteEventArgs e)
        {
            Log.Info($"Guild \"{e.Guild.Name}\" ({e.Guild.Id}) is now unavailable");
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
            var user = $"{e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})";
            Log.Info($"User {user} added to \"{e.Guild.Name}\" ({e.Guild.Id})");
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
            var user = $"{e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})";
            Log.Info($"User {user} removed from \"{e.Guild.Name}\" ({e.Guild.Id})");
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
            var user = $"{e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})";
            Log.Info($"User {user} banned from \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildBanRemoved(GuildBanRemoveEventArgs e)
        {
            var user = $"{e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})";
            Log.Info($"Ban on user {user} lifted from \"{e.Guild.Name}\" ({e.Guild.Id})");
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
            Log.Warning($"Client encountered an unhandled event: {e.EventName}. JSON: {e.Json}");
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
        
        private string SanitizeString(string input)
        {
            var output = input;
            output = output.Replace('\a', '\0');
            output = output.Replace('␇', '\0');
            output = output.Replace('⍾', '\0');
            output = output.Replace("\a", string.Empty);
            return output;
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
        
        private SolaceDiscordVoiceState ConvertVoiceState(DiscordVoiceState discord_state)
        {
            var state = new SolaceDiscordVoiceState()
            {
                User          = ConvertUser(discord_state.User),
                GuildId       = discord_state.Guild.Id,
                GuildName     = discord_state.Guild.Name,
                SelfDeafened  = discord_state.IsSelfDeafened,
                SelfMuted     = discord_state.IsSelfMuted,
                GuildDeafened = discord_state.IsServerDeafened,
                GuildMuted    = discord_state.IsServerMuted,
                Suppressed    = discord_state.IsSuppressed,
            };
            return state;
        }
        
        private SolaceDiscordUser ConvertUser(DiscordUser discord_user)
        {
            _ = int.TryParse(discord_user.Discriminator, out var discriminator);
            var user = new SolaceDiscordUser()
            {
                Username      = discord_user.Username,
                Discriminator = discriminator,
                Id            = discord_user.Id,
                IsBot         = discord_user.IsBot,
                AvatarHash    = discord_user.AvatarHash,
            };
            user.TrySetUrl(discord_user.AvatarUrl);
            return user;
        }
        
        private SolaceDiscordChannel ConvertChannel(DiscordChannel discord_channel)
        {
            var is_dm = discord_channel.Type.HasFlag(ChannelType.Private);
            var channel = new SolaceDiscordChannel()
            {
                Name      = discord_channel.Name,
                Id        = discord_channel.Id,
                GuildName = is_dm ? string.Empty : discord_channel.Guild.Name,
                GuildId   = is_dm ? 0L : discord_channel.GuildId,
            };
            return channel;
        }
        
        private SolaceDiscordEmoji ConvertEmoji(DiscordEmoji discord_emoji)
        {
            var emoji = new SolaceDiscordEmoji()
            {
                Name        = discord_emoji.Name,
                DiscordName = discord_emoji.GetDiscordName(),
                Id          = discord_emoji.Id,
            };
            emoji.TrySetUrl(discord_emoji.Url);
            return emoji;
        }
        
        private SolaceDiscordRole ConvertRole(DiscordRole discord_role, DiscordGuild guild)
        {
            var role = new SolaceDiscordRole()
            {
                GuildId   = guild.Id,
                GuildName = guild.Name,
                Name      = discord_role.Name,
                Id        = discord_role.Id,
            };
            return role;
        }
        
        private Task<SolaceDiscordMessage> ConvertMessage(DiscordMessage discord_message)
        {
            // this method is not async yet, but I'd like to have the opprotunity
            // in the future to do async calls in here, so it returns Task.
            
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
                Sender    = ConvertUser(discord_message.Author),
                IsDM      = is_dm,
                Nickname  = is_dm ? string.Empty : nickname,
                GuildName = is_dm ? string.Empty : discord_message.Channel.Guild.Name,
                GuildId   = is_dm ? 0L : discord_message.Channel.Guild.Id,
                Channel   = ConvertChannel(discord_message.Channel),
                MessageId = discord_message.Id,
                Message   = discord_message.Content,
            };
            
            message.Sender.TrySetUrl(discord_message.Author.AvatarUrl);
            
            foreach (var duser in discord_message.MentionedUsers)
            {
                _ = int.TryParse(discord_message.Author.Discriminator, out int user_discriminator);
                var user = ConvertUser(duser);
                message.MentionedUsers.Add(user);
            }
            
            foreach (var dchannel in discord_message.MentionedChannels)
            {
                var channel = ConvertChannel(dchannel);
                message.MentionedChannels.Add(channel);
            }
            
            foreach (var drole in discord_message.MentionedRoles)
            {
                var role = ConvertRole(drole, discord_message.Channel.Guild);
                message.MentionedRoles.Add(role);
            }
            
            foreach (var reaction in discord_message.Reactions)
            {
                var emoji = ConvertEmoji(reaction.Emoji);
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
