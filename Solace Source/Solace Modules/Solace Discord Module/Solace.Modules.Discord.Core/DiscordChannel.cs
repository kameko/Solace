
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class DiscordChannel
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public ulong GuildId { get; set; }
        
        public DiscordChannel()
        {
            Name = string.Empty;
        }
    }
}
