
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class DiscordRole
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        
        public DiscordRole()
        {
            Name = string.Empty;
        }
    }
}
