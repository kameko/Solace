
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
        public ulong GuildId { get; set; }
        public DiscordChannel Channel { get; set; }
        public ulong MessageId { get; set; }
        public string Message { get; set; }
        public List<DiscordUser> MentionedUsers { get; set; }
        public List<DiscordChannel> MentionedChannels { get; set; }
        public List<DiscordRole> MentionedRoles { get; set; }
        public List<Emoji> Reactions { get; set; }
        // TODO: attachments. will require some planning along with the database
        // to coordinate where we can find them on-disk.
        
        public DiscordMessage()
        {
            Sender      = new DiscordUser();
            Nickname    = string.Empty;
            GuildName   = string.Empty;
            Channel     = new DiscordChannel();
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
