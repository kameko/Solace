
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core;
    using Solace.Core.Services.Communication;
    
    public class SolaceDiscordMessage
    {
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Received { get; set; }
        public SolaceDiscordUser Sender { get; set; }
        public bool IsDM { get; set; }
        public string Nickname { get; set; }
        public SolaceDiscordGuild Guild { get; set; }
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
            Received = DateTime.UtcNow;
            Sender   = new SolaceDiscordUser();
            Nickname = string.Empty;
            Guild    = new SolaceDiscordGuild();
            Channel  = new SolaceDiscordChannel();
            Message  = string.Empty;
            
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
        
        public string GetMessageDifference(SolaceDiscordMessage other)
        {
            return this.GetMessageDifference(other);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is SolaceDiscordMessage other)
            {
                return Created   == other.Created
                    && Received  == other.Received
                    && Sender.Equals(other.Sender)
                    && IsDM      == other.IsDM
                    && Nickname  == other.Nickname
                    && Guild.Equals(other.Guild)
                    && Channel.Equals(other.Channel)
                    && MessageId == other.MessageId
                    && Message   == other.Message;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return new
            {
                Created,
                Received,
                Sender,
                IsDM,
                Nickname,
                Guild,
                Channel,
                MessageId,
                Message
            }
            .GetHashCode();
        }
    }
}
