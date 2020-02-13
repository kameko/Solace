
namespace Solace.Modules.Discord.Provider.DSharpPlus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
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
            Client.TypingStarted                  += ClientOnTypingStarted;
            Client.SocketOpened                   += ClientOnSocketOpened;
            Client.SocketErrored                  += ClientOnSocketError;
            Client.SocketClosed                   += ClientOnSocketClosed;
            Client.PresenceUpdated                += ClientOnPresenceUpdated;
            Client.MessageCreated                 += ClientOnMessageCreated;
            Client.MessageUpdated                 += (e) => Task.CompletedTask;
            Client.MessagesBulkDeleted            += (e) => Task.CompletedTask;
            Client.MessageReactionsCleared        += (e) => Task.CompletedTask;
            Client.MessageReactionRemoved         += (e) => Task.CompletedTask;
            Client.MessageReactionAdded           += (e) => Task.CompletedTask;
            Client.MessageDeleted                 += (e) => Task.CompletedTask;
            Client.MessageAcknowledged            += (e) => Task.CompletedTask;
            Client.Heartbeated                    += (e) => Task.CompletedTask;
            Client.GuildUpdated                   += (e) => Task.CompletedTask;
            Client.GuildUnavailable               += (e) => Task.CompletedTask;
            Client.GuildRoleUpdated               += (e) => Task.CompletedTask;
            Client.GuildRoleDeleted               += (e) => Task.CompletedTask;
            Client.GuildRoleCreated               += (e) => Task.CompletedTask;
            Client.GuildMemberUpdated             += (e) => Task.CompletedTask;
            Client.GuildMembersChunked            += (e) => Task.CompletedTask;
            Client.GuildMemberRemoved             += (e) => Task.CompletedTask;
            Client.GuildMemberAdded               += (e) => Task.CompletedTask;
            Client.GuildIntegrationsUpdated       += (e) => Task.CompletedTask;
            Client.GuildEmojisUpdated             += (e) => Task.CompletedTask;
            Client.GuildDownloadCompleted         += (e) => Task.CompletedTask;
            Client.GuildDeleted                   += (e) => Task.CompletedTask;
            Client.GuildCreated                   += (e) => Task.CompletedTask;
            Client.GuildBanRemoved                += (e) => Task.CompletedTask;
            Client.GuildBanAdded                  += (e) => Task.CompletedTask;
            Client.GuildAvailable                 += (e) => Task.CompletedTask;
            Client.DmChannelDeleted               += (e) => Task.CompletedTask;
            Client.DmChannelCreated               += (e) => Task.CompletedTask;
            Client.ChannelUpdated                 += (e) => Task.CompletedTask;
            Client.ChannelPinsUpdated             += (e) => Task.CompletedTask;
            Client.ChannelDeleted                 += (e) => Task.CompletedTask;
            Client.ChannelCreated                 += (e) => Task.CompletedTask;
            Client.UnknownEvent                   += (e) => Task.CompletedTask;
            
            return Task.CompletedTask;
        }
        
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
        
        private void CheckConfigured()
        {
            if (!Configured)
            {
                throw new InvalidOperationException("Provider is not configured yet.");
            }
        }
        
        private async Task ClientOnReady(ReadyEventArgs e)
        {
            Ready = true;
            await RaiseOnReady();
        }
        
        private Task ClientOnResume(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
        
        private Task ClientOnClientError(ClientErrorEventArgs e)
        {
            Log.Error(e.Exception, $"Event Error: {e.EventName}");
            return Task.CompletedTask;
        }
        
        private Task ClientOnWebhooksUpdated(WebhooksUpdateEventArgs e)
        {
            return Task.CompletedTask;
        }
        
        private Task ClientOnVoiceStateUpdated(VoiceStateUpdateEventArgs e)
        {
            return Task.CompletedTask;
        }
        
        private Task ClientOnVoiceServerUpdated(VoiceServerUpdateEventArgs e)
        {
            return Task.CompletedTask;
        }
        
        private Task ClientOnUserUpdated(UserUpdateEventArgs e)
        {
            return Task.CompletedTask;
        }
        
        private Task ClientOnUserSettingsUpdated(UserSettingsUpdateEventArgs e)
        {
            return Task.CompletedTask;
        }
        
        private Task ClientOnTypingStarted(TypingStartEventArgs e)
        {
            return Task.CompletedTask;
        }
        
        private Task ClientOnSocketOpened()
        {
            return Task.CompletedTask;
        }
        
        private Task ClientOnSocketError(SocketErrorEventArgs e)
        {
            return Task.CompletedTask;
        }
        
        private Task ClientOnSocketClosed(SocketCloseEventArgs e)
        {
            return Task.CompletedTask;
        }
        
        private Task ClientOnPresenceUpdated(PresenceUpdateEventArgs e)
        {
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
        
        private Task FUTUREEVENT11()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT12()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT13()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT14()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT15()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT16()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT17()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT18()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT19()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT20()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT21()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT22()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT23()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT24()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT25()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT26()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT27()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT28()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT29()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT30()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT31()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT32()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT33()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT34()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT35()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT36()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT37()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT38()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT39()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT40()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT41()
        {
            return Task.CompletedTask;
        }
        
        private Task FUTUREEVENT42()
        {
            return Task.CompletedTask;
        }
        
        private async Task ProcessOnTextMessageCreated(MessageCreateEventArgs discord_message)
        {
            var message = await ConvertMessage(discord_message.Message);
            await RaiseOnReceiveMessage(message);
        }
        
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
