
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core;
    
    public class SolaceDiscordChannel
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public SolaceDiscordGuild Guild { get; set; }
        
        public SolaceDiscordChannel()
        {
            Name  = string.Empty;
            Guild = new SolaceDiscordGuild();
        }
        
        public string GetChannelDifference(SolaceDiscordChannel other)
        {
            return ObjectExtensions.GetShallowObjectDifferencesAsString(this, other);
        }
        
        // TODO: override Equals
    }
}
