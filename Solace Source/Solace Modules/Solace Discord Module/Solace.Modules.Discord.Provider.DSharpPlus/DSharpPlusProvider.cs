
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
                int.TryParse(discord_message.Author.Discriminator, out int discriminator);
                var channel_sender = await discord_message.Channel.Guild.GetMemberAsync(discord_message.Author.Id);
                
                var message = new DiscordMessage()
                {
                    Sender    = new DiscordUser()
                    {
                        Username      = discord_message.Author.Username,
                        Discriminator = discriminator,
                        Id            = discord_message.Author.Id,
                        IsBot         = discord_message.Author.IsBot,
                    },
                    Nickname  = channel_sender.Nickname,
                    GuildName = discord_message.Guild.Name,
                    GuildId   = discord_message.Guild.Id,
                    Channel   = new DiscordChannel()
                    {
                        Name      = discord_message.Channel.Name,
                        Id        = discord_message.Channel.Id,
                        GuildName = discord_message.Channel.Guild.Name,
                        GuildId   = discord_message.Channel.GuildId,
                    },
                    MessageId = discord_message.Message.Id,
                    Message   = discord_message.Message.Content,
                };
                
                message.Sender.TrySetUrl(discord_message.Author.AvatarUrl);
                
                foreach (var user in discord_message.MentionedUsers)
                {
                    int.TryParse(discord_message.Author.Discriminator, out int user_discriminator);
                    
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
                    // TODO:
                    
                }
                
                await RaiseOnReceiveMessage(message);
            });
        }
    }
}
