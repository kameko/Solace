
namespace Solace.Core.Services.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class CommunicationMessage
    {
        public string Service { get; set; }
        public virtual DateTime Received { get; set; }
        public virtual string Sender { get; set; }
        public virtual string Location { get; set; }
        public virtual string Message { get; set; }
        public virtual IEnumerable<string> MentionedUsers { get; set; }
        
        public CommunicationMessage()
        {
            Service  = string.Empty;
            Received = DateTime.UtcNow;
            Sender   = string.Empty;
            Location = string.Empty;
            Message  = string.Empty;
            MentionedUsers = new List<string>();
        }
    }
}
