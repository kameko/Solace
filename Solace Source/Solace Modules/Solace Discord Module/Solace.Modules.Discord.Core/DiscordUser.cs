
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class DiscordUser
    {
        public string Username { get; set; }
        public int Discriminator { get; set; }
        public ulong Id { get; set; }
        public bool IsBot { get; set; }
        
        public DiscordUser()
        {
            Username = string.Empty;
        }
    }
}
