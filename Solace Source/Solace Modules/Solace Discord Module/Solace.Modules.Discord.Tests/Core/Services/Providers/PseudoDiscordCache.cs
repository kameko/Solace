
namespace Solace.Modules.Discord.Tests.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Modules.Discord.Core;
    
    public class PseudoDiscordCache
    {
        public PseudoDiscordUser User { get; set; }
        public IEnumerable<PseudoDiscordGuild> Guilds { get; set; }
        
        public PseudoDiscordCache()
        {
            User   = new PseudoDiscordUser();
            Guilds = new List<PseudoDiscordGuild>();
        }
    }
    
    public class PseudoDiscordGuild
    {
        public string Nickname { get; set; }
        public string GuildName { get; set; }
        public IEnumerable<PseudoDiscordUser> Users { get; set; }
        public IEnumerable<PseudoDiscordChannel> Channels { get; set; }
        
        public PseudoDiscordGuild()
        {
            Nickname  = string.Empty;
            GuildName = string.Empty;
            Users     = new List<PseudoDiscordUser>();
            Channels  = new List<PseudoDiscordChannel>();
        }
    }
    
    public class PseudoDiscordChannel
    {
        public IEnumerable<SolaceDiscordMessage> Messages { get; set; }
        
        public PseudoDiscordChannel()
        {
            Messages = new List<SolaceDiscordMessage>();
        }
    }
    
    public class PseudoDiscordUser
    {
        public string Username { get; set; }
        public int Discriminator { get; set; }
        public ulong UserId { get; set; }
        
        public PseudoDiscordUser()
        {
            Username = string.Empty;
        }
    }
}
