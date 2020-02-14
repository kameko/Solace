
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
    
    // Partial class split into DSharpPlusProvider_EventHandlers.cs
    public partial class DSharpPlusProvider : BaseDiscordProvider
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
            
            // Methods for these are in DSharpPlusProvider_EventHandlers.cs
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
        
        public override async Task StartTyping(ulong channel_id)
        {
            CheckConfigured();
            try
            {
                var channel = await Client.GetChannelAsync(channel_id);
                await channel.TriggerTypingAsync();
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
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
        
        public override async Task<bool> Send(ulong channel_id, string message, Stream resource, string filename)
        {
            CheckConfigured();
            try
            {
                var channel = await Client.GetChannelAsync(channel_id);
                await channel.SendFileAsync(filename, resource, message);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return false;
            }
        }
        
        public override async Task<bool> Send(ulong channel_id, Stream resource, string filename)
        {
            return await Send(channel_id, null!, resource, filename);
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
        
        public override async Task<SolaceDiscordGuild?> GetGuild(ulong guild_id)
        {
            CheckConfigured();
            try
            {
                var dguild = await Client.GetGuildAsync(guild_id);
                var guild  = await ConvertGuild(dguild);
                return guild;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return null;
            }
        }
        
        public override async Task<IEnumerable<SolaceDiscordGuild>> GetGuilds()
        {
            CheckConfigured();
            var guilds = new List<SolaceDiscordGuild>();
            try
            {
                foreach (var dguild in Client.Guilds)
                {
                    var guild = await ConvertGuild(dguild.Value);
                    guilds.Add(guild);
                }
                return guilds;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return guilds;
            }
        }
        
        public override async Task<SolaceDiscordChannel?> GetChannel(ulong channel_id)
        {
            CheckConfigured();
            try
            {
                var dchannel = await Client.GetChannelAsync(channel_id);
                var channel = await ConvertChannel(dchannel);
                return channel;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return null;
            }
        }
        
        public override async Task<IEnumerable<SolaceDiscordChannel>> GetChannels(ulong guild_id)
        {
            CheckConfigured();
            var channels = new List<SolaceDiscordChannel>();
            try
            {
                var guild = await Client.GetGuildAsync(guild_id);
                foreach (var dchannel in guild.Channels)
                {
                    var channel = await ConvertChannel(dchannel.Value);
                    channels.Add(channel);
                }
                return channels;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return channels;
            }
        }
        
        public override async Task<SolaceDiscordUser?> GetUser(ulong user_id)
        {
            CheckConfigured();
            try
            {
                var duser = await Client.GetUserAsync(user_id);
                var user  = await ConvertUser(duser);
                return user;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return null;
            }
        }
        
        public override async Task<IEnumerable<SolaceDiscordUser>> GetUsers(ulong guild_id)
        {
            CheckConfigured();
            var users = new List<SolaceDiscordUser>();
            try
            {
                var guild = await Client.GetGuildAsync(guild_id);
                foreach (var duser in await guild.GetAllMembersAsync())
                {
                    var user = await ConvertUser(duser);
                    users.Add(user);
                }
                return users;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return users;
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
            // create a task to constantly ping and
            // try to connect if it doesn't get a response.
            // needs to be configurable, but should default
            // to around a ping every two seconds. If it doesn't
            // get 3 pings in a row, disconnect and reconnect in
            // a loop.
            // Might not actually be needed, D#+ apparently handles this.
            
            // Do nothing.
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
            var index    = 1;
            foreach (var user in discord_users)
            {
                users_sb.Append($"{user.Username}#{user.Discriminator} ({user.Id})");
                if (!string.IsNullOrEmpty(user.Nickname) && user.Nickname != user.Username)
                {
                    users_sb.Append($" \"{user.Nickname}\"");
                }
                if (discord_users.Count() > 1 && index < discord_users.Count())
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
        
        private async Task<SolaceDiscordVoiceState> ConvertVoiceState(DiscordVoiceState discord_state)
        {
            var state = new SolaceDiscordVoiceState()
            {
                User          = await ConvertUser(discord_state.User),
                Guild         = await ConvertGuild(discord_state.Guild),
                SelfDeafened  = discord_state.IsSelfDeafened,
                SelfMuted     = discord_state.IsSelfMuted,
                GuildDeafened = discord_state.IsServerDeafened,
                GuildMuted    = discord_state.IsServerMuted,
                Suppressed    = discord_state.IsSuppressed,
            };
            return state;
        }
        
        private Task<SolaceDiscordRole> ConvertRole(DiscordRole discord_role)
        {
            var role = new SolaceDiscordRole()
            {
                Name = discord_role.Name,
                Id   = discord_role.Id,
            };
            return Task.FromResult(role);
        }
        
        private async Task<IEnumerable<SolaceDiscordRole>> ConvertRoles(IEnumerable<DiscordRole> discord_roles)
        {
            var list = new List<SolaceDiscordRole>(discord_roles.Count());
            foreach (var drole in discord_roles)
            {
                var roles = await ConvertRole(drole);
                list.Add(roles);
            }
            return list;
        }
        
        private async Task<SolaceDiscordUser> ConvertUser(DiscordUser discord_user)
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
            if (discord_user is DiscordMember member)
            {
                if (!string.IsNullOrEmpty(member.Nickname))
                {
                    user.Nickname = member.Nickname;
                }
                
                var roles = new List<SolaceDiscordRole>(member.Roles.Count());
                foreach (var drole in member.Roles)
                {
                    var role = await ConvertRole(drole);
                    roles.Add(role);
                }
                user.Roles = roles;
            }
            user.TrySetUrl(discord_user.AvatarUrl);
            return user;
        }
        
        private async Task<SolaceDiscordGuild> ConvertGuild(DiscordGuild discord_guild)
        {
            // Possibly add these as well:
            // discord_guild.GetInvitesAsync()
            // discord_guild.IconHash
            // discord_guild.IconUrl
            // discord_guild.JoinedAt
            // discord_guild.Roles
            
            var channels = new List<SolaceDiscordChannel>(discord_guild.Channels.Count());
            foreach (var dchannel in discord_guild.Channels)
            {
                var channel = await ConvertChannel(dchannel.Value);
                channels.Add(channel);
            }
            
            var dmembers = await discord_guild.GetAllMembersAsync();
            var members  = new List<SolaceDiscordUser>(dmembers.Count());
            foreach (var dmember in dmembers)
            {
                var member = await ConvertUser(dmember);
                members.Add(member);
            }
            
            var demojis = await discord_guild.GetEmojisAsync();
            var emojis  = new List<SolaceDiscordEmoji>(demojis.Count());
            foreach (var demoji in demojis)
            {
                var emoji = await ConvertEmoji(demoji);
                emojis.Add(emoji);
            }
            
            var dbans = await discord_guild.GetBansAsync();
            var bans  = new List<SolaceDiscordGuild.Ban>();
            foreach (var dban in dbans)
            {
                var user = await ConvertUser(dban.User);
                var ban  = new SolaceDiscordGuild.Ban(user, dban.Reason);
                bans.Add(ban);
            }
            
            var guild = new SolaceDiscordGuild()
            {
                Name           = discord_guild.Name,
                Id             = discord_guild.Id,
                Available      = !discord_guild.IsUnavailable,
                Channels       = channels,
                DefaultChannel = await ConvertChannel(discord_guild.GetDefaultChannel()),
                SystemChannel  = await ConvertChannel(discord_guild.SystemChannel),
                Owner          = await ConvertUser(discord_guild.Owner),
                Emojis         = emojis,
                Bans           = bans,
            };
            
            return guild;
        }
        
        private async Task<SolaceDiscordGuild> ConvertGuild(DiscordGuild discord_guild, bool is_dm)
        {
            if (is_dm)
            {
                var guild = new SolaceDiscordGuild()
                {
                    Name = string.Empty,
                    Id   = 0UL,
                };
                return guild;
            }
            else
            {
                return await ConvertGuild(discord_guild);
            }
        }
        
        private async Task<SolaceDiscordChannel> ConvertChannel(DiscordChannel discord_channel)
        {
            var is_dm = discord_channel.Type.HasFlag(ChannelType.Private);
            var channel = new SolaceDiscordChannel()
            {
                Name             = discord_channel.Name,
                Id               = discord_channel.Id,
                MentionString    = discord_channel.Mention,
                PerUserRateLimit = discord_channel.PerUserRateLimit ?? 0,
                Position         = discord_channel.Position,
                Topic            = discord_channel.Topic,
                Guild            = await ConvertGuild(discord_channel.Guild, is_dm),
            };
            return channel;
        }
        
        private Task<SolaceDiscordEmoji> ConvertEmoji(DiscordEmoji discord_emoji)
        {
            var emoji = new SolaceDiscordEmoji()
            {
                Name           = discord_emoji.Name,
                DiscordName    = discord_emoji.GetDiscordName(),
                Id             = discord_emoji.Id,
                RequiresColons = discord_emoji.RequiresColons,
            };
            emoji.TrySetUrl(discord_emoji.Url);
            return Task.FromResult(emoji);
        }
        
        private async Task<SolaceDiscordMessage> ConvertMessage(DiscordMessage discord_message)
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
                Sender    = await ConvertUser(discord_message.Author),
                IsDM      = is_dm,
                Nickname  = is_dm ? string.Empty : nickname,
                Guild     = await ConvertGuild(discord_message.Channel.Guild, is_dm),
                Channel   = await ConvertChannel(discord_message.Channel),
                MessageId = discord_message.Id,
                Message   = discord_message.Content,
            };
            
            message.Sender.TrySetUrl(discord_message.Author.AvatarUrl);
            
            foreach (var duser in discord_message.MentionedUsers)
            {
                _ = int.TryParse(discord_message.Author.Discriminator, out int user_discriminator);
                var user = await ConvertUser(duser);
                message.MentionedUsers.Add(user);
            }
            
            foreach (var dchannel in discord_message.MentionedChannels)
            {
                var channel = await ConvertChannel(dchannel);
                message.MentionedChannels.Add(channel);
            }
            
            foreach (var drole in discord_message.MentionedRoles)
            {
                var role = await ConvertRole(drole);
                message.MentionedRoles.Add(role);
            }
            
            foreach (var reaction in discord_message.Reactions)
            {
                var emoji = await ConvertEmoji(reaction.Emoji);
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
            
            return message;
        }
        
        public override async ValueTask DisposeAsync()
        {
            await Disconnect();
        }
    }
}
