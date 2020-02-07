
namespace Solace.Modules.Discord.Provider.DSharpPlus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Services.Providers;
    using global::DSharpPlus;
    using global::DSharpPlus.EventArgs;
    
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
            await Client.ConnectAsync();
        }
        
        private Task OnMessageCreated(MessageCreateEventArgs discord_message)
        {
            if (discord_message.Author.IsCurrent)
            {
                return Task.CompletedTask;
            }
            
            return Task.Run(async () =>
            {
                var discriminator = 0;
                var is_dm = false;
                var nickname = string.Empty;
                
                _ = int.TryParse(discord_message.Author.Discriminator, out discriminator);
                if (discord_message.Channel.Type.HasFlag(ChannelType.Private))
                {
                    is_dm = true;
                }
                else
                {
                    var channel_sender = await discord_message.Channel.Guild.GetMemberAsync(discord_message.Author.Id);
                    is_dm = false;
                    nickname = channel_sender?.Nickname ?? string.Empty;
                    if (string.IsNullOrEmpty(nickname))
                    {
                        nickname = string.Empty;
                    }
                }
                
                var message = new DiscordMessage()
                {
                    Created   = discord_message.Message.CreationTimestamp,
                    Sender    = new DiscordUser()
                    {
                        Username      = discord_message.Author.Username,
                        Discriminator = discriminator,
                        Id            = discord_message.Author.Id,
                        IsBot         = discord_message.Author.IsBot,
                    },
                    IsDM      = is_dm,
                    Nickname  = is_dm ? string.Empty : nickname,
                    GuildName = is_dm ? string.Empty : discord_message.Guild.Name,
                    GuildId   = is_dm ? 0L : discord_message.Guild.Id,
                    Channel   = new DiscordChannel()
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
                    
                    var nuser = new DiscordUser()
                    {
                        Username      = user.Username,
                        Discriminator = user_discriminator,
                        Id            = user.Id,
                        IsBot         = user.IsBot,
                    };
                    
                    nuser.TrySetUrl(user.AvatarUrl);
                    
                    message.MentionedUsers.Add(nuser);
                }
                
                foreach (var channel in discord_message.MentionedChannels)
                {
                    var nchannel = new DiscordChannel()
                    {
                        Name    = channel.Name,
                        Id      = channel.Id,
                        GuildId = channel.GuildId,
                    };
                    
                    message.MentionedChannels.Add(nchannel);
                }
                
                foreach (var role in discord_message.MentionedRoles)
                {
                    var nrole = new DiscordRole()
                    {
                        Name = role.Name,
                        Id   = role.Id,
                    };
                    
                    message.MentionedRoles.Add(nrole);
                }
                
                foreach (var reaction in discord_message.Message.Reactions)
                {
                    var emoji = new Emoji()
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
            });
        }
    }
}
