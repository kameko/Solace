
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core.Services.Communication;
    
    public class DiscordMessage
    {
        public DiscordUser Sender { get; set; }
        public string Nickname { get; set; }
        public string GuildName { get; set; }
        public ulong GuildSnowflake { get; set; }
        public string ChannelName { get; set; }
        public ulong ChannelSnowflake { get; set; }
        public ulong MessageId { get; set; }
        public string Message { get; set; }
        public List<DiscordUser> MentionedUsers { get; set; }
        public List<DiscordChannel> MentionedChannels { get; set; }
        public List<DiscordRole> MentionedRoles { get; set; }
        public List<Emoji> Reactions { get; set; }
        
        public DiscordMessage()
        {
            Sender      = new DiscordUser();
            Nickname    = string.Empty;
            GuildName   = string.Empty;
            ChannelName = string.Empty;
            Message     = string.Empty;
            
            MentionedUsers    = new List<DiscordUser>();
            MentionedChannels = new List<DiscordChannel>();
            MentionedRoles    = new List<DiscordRole>();
            Reactions         = new List<Emoji>();
        }
        
        public CommunicationMessage ToCommunicationMessage()
        {
            throw new NotImplementedException();
        }
    }
}
