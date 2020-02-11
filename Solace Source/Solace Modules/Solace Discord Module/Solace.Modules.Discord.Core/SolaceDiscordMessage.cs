
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core.Services.Communication;
    using Services;
    
    public class SolaceDiscordMessage
    {
        internal DiscordService? DiscordService { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Received { get; set; }
        public SolaceDiscordUser Sender { get; set; }
        public bool IsDM { get; set; }
        public string Nickname { get; set; }
        public string GuildName { get; set; }
        public ulong GuildId { get; set; }
        public SolaceDiscordChannel Channel { get; set; }
        public ulong MessageId { get; set; }
        public string Message { get; set; }
        
        public List<SolaceDiscordUser> MentionedUsers { get; set; }
        public List<SolaceDiscordChannel> MentionedChannels { get; set; }
        public List<SolaceDiscordRole> MentionedRoles { get; set; }
        public List<SolaceDiscordEmoji> Reactions { get; set; }
        public List<AttachmentToken> Attachments { get; set; }
        
        public SolaceDiscordMessage()
        {
            Received    = DateTime.UtcNow;
            Sender      = new SolaceDiscordUser();
            Nickname    = string.Empty;
            GuildName   = string.Empty;
            Channel     = new SolaceDiscordChannel();
            Message     = string.Empty;
            
            MentionedUsers    = new List<SolaceDiscordUser>();
            MentionedChannels = new List<SolaceDiscordChannel>();
            MentionedRoles    = new List<SolaceDiscordRole>();
            Reactions         = new List<SolaceDiscordEmoji>();
            Attachments       = new List<AttachmentToken>();
        }
        
        public CommunicationMessage ToCommunicationMessage()
        {
            throw new NotImplementedException();
        }
    }
}
