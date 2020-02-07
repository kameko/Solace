
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class SolaceDiscordChannel
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public string GuildName { get; set; }
        public ulong GuildId { get; set; }
        
        public SolaceDiscordChannel()
        {
            Name      = string.Empty;
            GuildName = string.Empty;
        }
    }
}
