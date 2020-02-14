
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
        public IEnumerable<SolaceDiscordChannel> Channels { get; set; }
        public SolaceDiscordUser Owner { get; set; }
        
        public SolaceDiscordGuild()
        {
            Name     = string.Empty;
            Channels = new List<SolaceDiscordChannel>();
            Owner    = new SolaceDiscordUser();
        }
        
        public string GetGuildDifference(SolaceDiscordGuild other)
        {
            return ObjectExtensions.GetShallowObjectDifferencesAsString(this, other);
        }
        
        // TODO: override Equals
    }
}
