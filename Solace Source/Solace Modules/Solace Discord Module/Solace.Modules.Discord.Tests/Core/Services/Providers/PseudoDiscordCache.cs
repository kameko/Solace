
namespace Solace.Modules.Discord.Tests.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Modules.Discord.Core;
    
    public class PseudoDiscordCache
    {
        public SolaceDiscordUser User { get; set; }
        public List<PseudoDiscordGuild> Guilds { get; set; }
        
        public PseudoDiscordCache()
        {
            User   = new SolaceDiscordUser();
            Guilds = new List<PseudoDiscordGuild>();
        }
    }
    
    public class PseudoDiscordGuild
    {
        public string Nickname { get; set; }
        public string Name { get; set; }
        public ulong Id { get; set; }
        public List<SolaceDiscordUser> Users { get; set; }
        public List<PseudoDiscordChannel> Channels { get; set; }
        
        public PseudoDiscordGuild()
        {
            Nickname  = string.Empty;
            Name      = string.Empty;
            Users     = new List<SolaceDiscordUser>();
            Channels  = new List<PseudoDiscordChannel>();
        }
    }
    
    public class PseudoDiscordChannel
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public List<SolaceDiscordMessage> Messages { get; set; }
        
        public PseudoDiscordChannel()
        {
            Name     = string.Empty;
            Messages = new List<SolaceDiscordMessage>();
        }
    }
}
