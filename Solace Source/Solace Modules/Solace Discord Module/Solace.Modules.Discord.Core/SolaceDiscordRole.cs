
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class SolaceDiscordRole
    {
        public SolaceDiscordGuild Guild { get; set; }
        public string Name { get; set; }
        public ulong Id { get; set; }
        
        public SolaceDiscordRole()
        {
            Guild = new SolaceDiscordGuild();
            Name  = string.Empty;
        }
        
        // TODO: override Equals
    }
}
