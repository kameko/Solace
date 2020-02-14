
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public static class DifferenceTokens
    {
        public interface IDifference
        {
            string GetDifferenceString();
        }
        
        public abstract class BaseDifference : IDifference
        {
            public abstract string GetDifferenceString();
            
            public override string ToString()
            {
                return GetDifferenceString();
            }
        }
        
        public class VoiceStateDifference : BaseDifference
        {
            public SolaceDiscordVoiceState Before { get; set; }
            public SolaceDiscordVoiceState After { get; set; }
            
            public VoiceStateDifference(SolaceDiscordVoiceState before, SolaceDiscordVoiceState after)
            {
                Before = before;
                After  = after;
            }
            
            public override string GetDifferenceString()
            {
                return Before.GetDifferenceString(After);
            }
        }
        
        public class UserUpdatedDifference : BaseDifference
        {
            public SolaceDiscordUser Before { get; set; }
            public SolaceDiscordUser After { get; set; }
            
            public UserUpdatedDifference(SolaceDiscordUser before, SolaceDiscordUser after)
            {
                Before = before;
                After  = after;
            }
            
            public override string GetDifferenceString()
            {
                return Before.GetUserDifference(After);
            }
        }
        
        public class PresenceUpdatedDifference : BaseDifference
        {
            
            
            public override string GetDifferenceString()
            {
                return string.Empty;
            }
        }
    }
}
