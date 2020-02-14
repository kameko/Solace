
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core;
    
    public class SolaceDiscordGuild
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public bool Available { get; set; }
        public IEnumerable<SolaceDiscordChannel> Channels { get; set; }
        public SolaceDiscordChannel DefaultChannel { get; set; }
        public SolaceDiscordChannel SystemChannel { get; set; }
        public SolaceDiscordUser Owner { get; set; }
        public IEnumerable<SolaceDiscordEmoji> Emojis { get; set; }
        public IEnumerable<Ban> Bans { get; set; }
        
        public SolaceDiscordGuild()
        {
            Name           = string.Empty;
            Channels       = new List<SolaceDiscordChannel>();
            DefaultChannel = new SolaceDiscordChannel();
            SystemChannel  = new SolaceDiscordChannel();
            Owner          = new SolaceDiscordUser();
            Emojis         = new List<SolaceDiscordEmoji>();
            Bans           = new List<Ban>();
        }
        
        public string GetGuildDifference(SolaceDiscordGuild other)
        {
            return ObjectExtensions.GetShallowObjectDifferencesAsString(this, other);
        }
        
        // TODO: override Equals
        
        public class Ban
        {
            public SolaceDiscordUser User { get; set; }
            public string Reason { get; set; }
            
            public Ban(SolaceDiscordUser user, string reason)
            {
                User   = user;
                Reason = reason;
            }
        }
    }
}
