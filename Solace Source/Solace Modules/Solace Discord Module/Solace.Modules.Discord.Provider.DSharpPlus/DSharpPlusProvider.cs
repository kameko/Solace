
namespace Solace.Modules.Discord.Provider.DSharpPlus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        
        public DSharpPlusProvider() : base()
        {
            Client = null!;
        }
        
        public override Task Setup(string token)
        {
            return Task.Run(() =>
            {
                Client = new DiscordClient(new DiscordConfiguration
                {
                    Token     = token,
                    TokenType = TokenType.Bot,
                });
                
                Client.MessageCreated += OnMessageCreated;
            });
        }
        
        public override async Task Connect()
        {
            if (Connected)
            {
                return;
            }
            
            await Client.ConnectAsync();
            Connected = true;
            // TODO: create a task to constantly ping and
            // try to connect if it doesn't get a response.
            // needs to be configurable, but should default
            // to around a ping every two seconds. If it doesn't
            // get 3 pings in a row, disconnect and reconnect in
            // a loop.
        }
        
        public override async Task Disconnect()
        {
            if (!Connected)
            {
                return;
            }
            
            Connected = false;
            await Client.DisconnectAsync();
        }
        
        private Task PingLoop(CancellationToken token, int timeout, int tries)
        {
            throw new NotImplementedException();
        }
        
        private Task OnMessageCreated(MessageCreateEventArgs discord_message)
        {
            if (discord_message.Author.IsCurrent)
            {
                return Task.CompletedTask;
            }
            
            return Task.Run(async () =>
            {
                if (discord_message.Channel.Type.HasFlag(ChannelType.Text))
                {
                    try
                    {
                        await OnTextMessageCreated(discord_message);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, string.Empty);
                    }
                }
                else
                {
                    Log.Warning($"Unhandled Discord message type: {discord_message.Channel.Type}");
                }
            });
        }
        
        private async Task OnTextMessageCreated(MessageCreateEventArgs discord_message)
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
                Created   = discord_message.Message.CreationTimestamp,
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
                GuildName = is_dm ? string.Empty : discord_message.Guild.Name,
                GuildId   = is_dm ? 0L : discord_message.Guild.Id,
                Channel   = new SolaceDiscordChannel()
                {
                    Name      = discord_message.Channel.Name,
                    Id        = discord_message.Channel.Id,
                    GuildName = is_dm ? string.Empty : discord_message.Channel.Guild.Name,
                    GuildId   = is_dm ? 0L : discord_message.Channel.GuildId,
                },
                MessageId = discord_message.Message.Id,
                Message   = discord_message.Message.Content,
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
            
            foreach (var reaction in discord_message.Message.Reactions)
            {
                var emoji = new SolaceEmoji()
                {
                    Name = reaction.Emoji.Name,
                    Id   = reaction.Emoji.Id,
                };
                
                emoji.TrySetUrl(reaction.Emoji.Url);
                
                message.Reactions.Add(emoji);
            }
            
            foreach (var attachment in discord_message.Message.Attachments)
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
            
            await RaiseOnReceiveMessage(message);
        }
        
        public override async ValueTask DisposeAsync()
        {
            await Disconnect();
        }
    }
}
