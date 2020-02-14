
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    // TODO: replace all instances of "ulong GuildId, string GuildName" with this
    
    public class SolaceDiscordGuild
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        
        public SolaceDiscordGuild()
        {
            Name = string.Empty;
        }
        
        // TODO: Equals
    }
}
