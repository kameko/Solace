
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core;
    
    public class SolaceDiscordVoiceState
    {
        public SolaceDiscordUser User { get; set; }
        public SolaceDiscordGuild Guild { get; set; }
        public bool SelfDeafened { get; set; }
        public bool SelfMuted { get; set; }
        public bool GuildDeafened { get; set; }
        public bool GuildMuted { get; set; }
        public bool Suppressed { get; set; }
        
        public SolaceDiscordVoiceState()
        {
            User  = new SolaceDiscordUser();
            Guild = new SolaceDiscordGuild();
        }
        
        public string GetDifferenceString(SolaceDiscordVoiceState other)
        {
            return ObjectExtensions.GetShallowObjectDifferencesAsString(this, other);
        }
        
        // TODO: override Equals
    }
}
