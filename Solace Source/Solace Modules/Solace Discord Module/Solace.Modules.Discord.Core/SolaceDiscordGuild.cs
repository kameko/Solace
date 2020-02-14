
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
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
        
        // TODO: override Equals
    }
}
