
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class SolaceDiscordRole
    {
        public ulong GuildId { get; set; }
        public string GuildName { get; set; }
        public string Name { get; set; }
        public ulong Id { get; set; }
        
        public SolaceDiscordRole()
        {
            GuildName = string.Empty;
            Name      = string.Empty;
        }
        
        // TODO: override Equals
    }
}
