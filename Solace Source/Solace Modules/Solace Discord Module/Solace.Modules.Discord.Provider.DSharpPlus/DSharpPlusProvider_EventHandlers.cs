
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
    
    // Partial class split into DSharpPlusProvider.cs
    public partial class DSharpPlusProvider
    {
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
            var before = await ConvertVoiceState(e.Before);
            var after  = await ConvertVoiceState(e.After);
            var diff   = new DifferenceTokens.VoiceStateDifference(before, after);
            
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
            var before = await ConvertUser(e.UserBefore);
            var after  = await ConvertUser(e.UserAfter);
            var diff   = new DifferenceTokens.UserDifference(before, after);
            
            var user   = $"{e.UserAfter.Username}#{e.UserAfter.Discriminator} ({e.UserAfter.Id})";
            
            Log.Info($"User {user} updated. Differences: {diff}");
            
            await RaiseOnUserUpdated(diff);
        }
        
        private async Task ClientOnUserSettingsUpdated(UserSettingsUpdateEventArgs e)
        {
            var user = await ConvertUser(e.User);
            
            await RaiseOnUserSettingsUpdated(user);
        }
        
        private async Task ClientOnPresenceUpdated(PresenceUpdateEventArgs e)
        {
            var before_user   = await ConvertUser(e.UserBefore);
            var after_user    = await ConvertUser(e.UserAfter);
            var user_diff     = new DifferenceTokens.UserDifference(before_user, after_user);
            
            // TODO: finish the presence class
            var user          = await ConvertUser(e.User);
            var presence_diff = new DifferenceTokens.PresenceDifference(user);
            
            var diff = new DifferenceTokens.PresenceUpdatedDifference(presence_diff, user_diff);
            
            await RaiseOnPresenceUpdated(diff);
        }
        
        private async Task ClientOnTypingStarted(TypingStartEventArgs e)
        {
            var user    = await ConvertUser(e.User);
            var channel = await ConvertChannel(e.Channel);
            
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
                var is_dm   = e.Channel.Type.HasFlag(ChannelType.Private);
                var source  = is_dm ? $"DM {e.Channel.Id}" : $"\"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}";
                var user    = $"{e.Author.Username}#{e.Author.Discriminator} ({e.Author.Id})";
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
            var after   = await ConvertMessage(e.Message);
            var diff    = new DifferenceTokens.MessageDifference(before, after);
            
            var is_dm   = e.Channel.Type.HasFlag(ChannelType.Private);
            var source  = is_dm ? $"DM {e.Channel.Id}" : $"\"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}";
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
            var is_dm   = e.Channel.Type.HasFlag(ChannelType.Private);
            var source  = is_dm ? $"DM {e.Channel.Id}" : $"\"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}";
            var user    = $"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id})";
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
        
        private async Task ClientOnMessagesBulkDeleted(MessageBulkDeleteEventArgs e)
        {
            var is_dm  = e.Channel.Type.HasFlag(ChannelType.Private);
            var source = is_dm ? $"DM {e.Channel.Id}" : $"\"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id}";
            Log.Info($"Bulk message deletion in {source}. Number of deleted messages: {e.Messages.Count()}");
            
            var channel = await ConvertChannel(e.Channel);
            var messages = new List<SolaceDiscordMessage>(e.Messages.Count());
            
            foreach (var deleted in e.Messages)
            {
                var message = await ConvertMessage(deleted);
                messages.Add(message);
            }
            
            await RaiseOnBulkMessageDeletion(channel, messages);
        }
        
        private async Task ClientOnMessageReactionAdded(MessageReactionAddEventArgs e)
        {
            Log.Info(
                $"Message reaction {e.Emoji.GetDiscordName()} added for message "
              + $"{e.Message.Id} \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})"
              + (string.IsNullOrEmpty(e.Message.Content) ? string.Empty : $". Content: {SanitizeString(e.Message.Content)}. End Content")
            );
            
            var message = await ConvertMessage(e.Message);
            var user    = await ConvertUser(e.User);
            var emoji   = await ConvertEmoji(e.Emoji);
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
            var emoji   = await ConvertEmoji(e.Emoji);
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
        
        private async Task ClientOnDmChannelCreated(DmChannelCreateEventArgs e)
        {
            Log.Info($"DM {e.Channel.Id} with {ConcatMembers(e.Channel.Users)} created");
            
            var channel = await ConvertChannel(e.Channel);
            var users   = new List<SolaceDiscordUser>(e.Channel.Users.Count());
            foreach (var duser in e.Channel.Users)
            {
                var user = await ConvertUser(duser);
                users.Add(user);
            }
            await RaiseOnDmCreated(channel, users);
        }
        
        private async Task ClientOnDmChannelDeleted(DmChannelDeleteEventArgs e)
        {
            Log.Info($"DM {e.Channel.Id} with {ConcatMembers(e.Channel.Users)} deleted");
            
            var channel = await ConvertChannel(e.Channel);
            var users   = new List<SolaceDiscordUser>(e.Channel.Users.Count());
            foreach (var duser in e.Channel.Users)
            {
                var user = await ConvertUser(duser);
                users.Add(user);
            }
            await RaiseOnDmDeleted(channel, users);
        }
        
        private async Task ClientOnChannelCreated(ChannelCreateEventArgs e)
        {
            Log.Info($"Channel {e.Channel.Name} ({e.Channel.Id}) created in \"{e.Guild.Name}\" ({e.Guild.Id})");
            
            var channel = await ConvertChannel(e.Channel);
            await RaiseOnChannelCreated(channel);
        }
        
        private async Task ClientOnChannelUpdated(ChannelUpdateEventArgs e)
        {
            var before = await ConvertChannel(e.ChannelBefore);
            var after  = await ConvertChannel(e.ChannelAfter);
            var diff   = new DifferenceTokens.ChannelDifference(before, after);
            
            Log.Info(
                $"Channel \"{e.Guild.Name}\"\\{e.ChannelAfter.Name} ({e.Guild.Id}\\{e.ChannelAfter.Id})"
              + $"updated. Differences: {diff}"
            );
            
            await RaiseOnChannelUpdated(diff);
        }
        
        private async Task ClientOnChannelPinsUpdated(ChannelPinsUpdateEventArgs e)
        {
            Log.Info($"Pins updated in \"{e.Guild.Name}\"\\{e.Channel.Name} ({e.Guild.Id}\\{e.Channel.Id})");
            
            var channel = await ConvertChannel(e.Channel);
            await RaiseOnChannelPinsUpdated(channel, e.LastPinTimestamp);
        }
        
        private async Task ClientOnChannelDeleted(ChannelDeleteEventArgs e)
        {
            Log.Info($"Channel {e.Channel.Name} ({e.Channel.Id}) deleted in \"{e.Guild.Name}\" ({e.Guild.Id})");
            
            var channel = await ConvertChannel(e.Channel);
            await RaiseOnChannelDeleted(channel);
        }
        
        private async Task ClientOnGuildCreated(GuildCreateEventArgs e)
        {
            Log.Info($"Guild \"{e.Guild.Name}\" ({e.Guild.Id}) created");
            
            e.Guild.RequestAllMembers();
            
            var guild = await ConvertGuild(e.Guild);
            
            await RaiseOnGuildCreated(guild);
        }
        
        private async Task ClientOnGuildUpdated(GuildUpdateEventArgs e)
        {
            var before = await ConvertGuild(e.GuildBefore);
            var after  = await ConvertGuild(e.GuildAfter);
            var diff   = new DifferenceTokens.GuildDifference(before, after);
            
            Log.Info($"Guild \"{e.GuildAfter.Name}\" ({e.GuildAfter.Id}) updated. Differences: {diff}");
            
            await RaiseOnGuildUpdated(diff);
        }
        
        private async Task ClientOnGuildDeleted(GuildDeleteEventArgs e)
        {
            Log.Info($"Guild \"{e.Guild.Name}\" ({e.Guild.Id}) deleted");
            
            var guild = await ConvertGuild(e.Guild);
            
            await RaiseOnGuildDeleted(guild);
        }
        
        private async Task ClientOnGuildAvailable(GuildCreateEventArgs e)
        {
            Log.Info($"Guild \"{e.Guild.Name}\" ({e.Guild.Id}) is now available");
            
            e.Guild.RequestAllMembers();
            
            var guild = await ConvertGuild(e.Guild);
            
            await RaiseOnGuildAvailable(guild);
        }
        
        private async Task ClientOnGuildUnavailable(GuildDeleteEventArgs e)
        {
            Log.Info($"Guild \"{e.Guild.Name}\" ({e.Guild.Id}) is now unavailable");
            
            var guild = await ConvertGuild(e.Guild);
            
            await RaiseOnGuildUnavailable(guild);
        }
        
        private Task ClientOnGuildDownloadComplete(GuildDownloadCompletedEventArgs e)
        {
            // unsure right now
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildRoleCreated(GuildRoleCreateEventArgs e)
        {
            Log.Info($"Role \"{e.Role.Name}\" ({e.Role.Id}) created in \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: unsure
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildRoleUpdated(GuildRoleUpdateEventArgs e)
        {
            // TODO: unsure
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildRoleDeleted(GuildRoleDeleteEventArgs e)
        {
            Log.Info($"Role \"{e.Role.Name}\" ({e.Role.Id}) deleted in \"{e.Guild.Name}\" ({e.Guild.Id})");
            // TODO: unsure
            return Task.CompletedTask;
        }
        
        private async Task ClientOnGuildMemberAdded(GuildMemberAddEventArgs e)
        {
            var user = $"{e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})";
            if (!string.IsNullOrEmpty(e.Member.Nickname) && e.Member.Nickname != e.Member.Username)
            {
                user += $" \"{e.Member.Nickname}\"";
            }
            Log.Info($"User {user} added to \"{e.Guild.Name}\" ({e.Guild.Id})");
            
            var guild = await ConvertGuild(e.Guild);
            var suser = await ConvertUser(e.Member);
            
            await RaiseOnGuildUserAdded(guild, suser);
        }
        
        private async Task ClientOnGuildMemberUpdated(GuildMemberUpdateEventArgs e)
        {
            var duser         = await ConvertUser(e.Member);
            var guild         = await ConvertGuild(e.Guild);
            
            var roles_before  = await ConvertRoles(e.RolesBefore);
            var roles_after   = await ConvertRoles(e.RolesAfter);
            
            var added_roles   = roles_after.Except(roles_before);
            var removed_roles = roles_before.Except(roles_after);
            
            var diff          = new DifferenceTokens.GuildUserDifference(
                duser, guild, e.NicknameBefore, e.NicknameAfter, added_roles, removed_roles
            );
            
            var user = $"{e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})";
            if (!string.IsNullOrEmpty(e.Member.Nickname) && e.Member.Nickname != e.Member.Username)
            {
                user += $" \"{e.Member.Nickname}\"";
            }
            Log.Info($"User {user} updated in \"{e.Guild.Name}\" ({e.Guild.Id}). Differences: {diff}");
            
            await RaiseOnGuildUserUpdated(diff);
        }
        
        private async Task ClientOnGuildMemberRemoved(GuildMemberRemoveEventArgs e)
        {
            var user = $"{e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})";
            if (!string.IsNullOrEmpty(e.Member.Nickname) && e.Member.Nickname != e.Member.Username)
            {
                user += $" \"{e.Member.Nickname}\"";
            }
            Log.Info($"User {user} removed from \"{e.Guild.Name}\" ({e.Guild.Id})");
            
            var guild = await ConvertGuild(e.Guild);
            var suser = await ConvertUser(e.Member);
            
            await RaiseOnGuildUserRemoved(guild, suser);
        }
        
        private Task ClientOnGuildMembersChunked(GuildMembersChunkEventArgs e)
        {
            // nothing right now.
            return Task.CompletedTask;
        }
        
        private async Task ClientOnGuildBanAdded(GuildBanAddEventArgs e)
        {
            var user = $"{e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})";
            Log.Info($"User {user} banned from \"{e.Guild.Name}\" ({e.Guild.Id})");
            
            var guild = await ConvertGuild(e.Guild);
            var duser = await ConvertUser(e.Member);
            
            await RaiseOnGuildUserBanned(guild, duser);
        }
        
        private async Task ClientOnGuildBanRemoved(GuildBanRemoveEventArgs e)
        {
            var user = $"{e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})";
            Log.Info($"Ban on user {user} lifted from \"{e.Guild.Name}\" ({e.Guild.Id})");
            
            var guild = await ConvertGuild(e.Guild);
            var duser = await ConvertUser(e.Member);
            
            await RaiseOnGuildUserUnbanned(guild, duser);
        }
        
        private Task ClientOnGuildEmojisUpdated(GuildEmojisUpdateEventArgs e)
        {
            // TODO: before/after
            // TODO: event for this
            return Task.CompletedTask;
        }
        
        private Task ClientOnGuildIntegrationsUpdated(GuildIntegrationsUpdateEventArgs e)
        {
            // unsure right now
            return Task.CompletedTask;
        }
        
        private Task ClientOnUnknownEvent(UnknownEventArgs e)
        {
            Log.Warning($"Client encountered an unhandled event: {e.EventName}. JSON: {e.Json}");
            return Task.CompletedTask;
        }
    }
}
