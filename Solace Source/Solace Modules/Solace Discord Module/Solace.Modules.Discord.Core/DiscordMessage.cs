
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core.Services.Communication;
    
    public class DiscordMessage
    {
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Received { get; set; }
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
        public List<AttachmentToken> Attachments { get; set; }
        
        public DiscordMessage()
        {
            Received    = DateTime.UtcNow;
            Sender      = new DiscordUser();
            Nickname    = string.Empty;
            GuildName   = string.Empty;
            Channel     = new DiscordChannel();
            Message     = string.Empty;
            
            MentionedUsers    = new List<DiscordUser>();
            MentionedChannels = new List<DiscordChannel>();
            MentionedRoles    = new List<DiscordRole>();
            Reactions         = new List<Emoji>();
            Attachments       = new List<AttachmentToken>();
        }
        
        public CommunicationMessage ToCommunicationMessage()
        {
            throw new NotImplementedException();
        }
    }
}
