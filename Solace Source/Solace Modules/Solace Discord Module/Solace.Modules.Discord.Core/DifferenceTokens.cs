
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
        
        public abstract class BaseDifference<T> : BaseDifference
        {
            public T Before { get; set; }
            public T After { get; set; }
            
            public BaseDifference(T before, T after)
            {
                Before = before;
                After  = after;
            }
        }
        
        public class VoiceStateDifference : BaseDifference<SolaceDiscordVoiceState>
        {
            public VoiceStateDifference(SolaceDiscordVoiceState before, SolaceDiscordVoiceState after) : base(before, after)
            {
                
            }
            
            public override string GetDifferenceString()
            {
                return Before.GetDifferenceString(After);
            }
        }
        
        public class UserDifference : BaseDifference<SolaceDiscordUser>
        {
            public UserDifference(SolaceDiscordUser before, SolaceDiscordUser after) : base(before, after)
            {
                
            }
            
            public override string GetDifferenceString()
            {
                return Before.GetUserDifference(After);
            }
        }
        
        public class PresenceDifference : BaseDifference
        {
            // TODO: SolaceDiscordPresence class, I guess
            public SolaceDiscordUser User { get; set; }
            
            public PresenceDifference(SolaceDiscordUser user)
            {
                User = user;
            }
            
            public override string GetDifferenceString()
            {
                return string.Empty;
            }
        }
        
        public class PresenceUpdatedDifference : BaseDifference
        {
            public PresenceDifference PresenceDifference { get; set; }
            public UserDifference UserDifference { get; set; }
            
            public PresenceUpdatedDifference(PresenceDifference presence_diff, UserDifference user_diff)
            {
                PresenceDifference = presence_diff;
                UserDifference     = user_diff;
            }
            
            public override string GetDifferenceString()
            {
                var diff = PresenceDifference.GetDifferenceString();
                if (!string.IsNullOrEmpty(diff))
                {
                    diff = $"Presence: {diff}";
                    var udiff = UserDifference.GetDifferenceString();
                    if (!string.IsNullOrEmpty(udiff))
                    {
                        diff += $"User: {udiff}";
                    }
                }
                return diff;
            }
        }
        
        public class MessageDifference : BaseDifference<SolaceDiscordMessage>
        {
            public MessageDifference(SolaceDiscordMessage? before, SolaceDiscordMessage after) : base(before!, after)
            {
                
            }
            
            public override string GetDifferenceString()
            {
                return Before?.GetMessageDifference(After) ?? string.Empty;
            }
        }
    }
}
