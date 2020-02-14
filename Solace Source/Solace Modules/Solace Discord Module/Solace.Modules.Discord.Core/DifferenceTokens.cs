
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text;
    
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
        
        public class ChannelDifference : BaseDifference<SolaceDiscordChannel>
        {
            public ChannelDifference(SolaceDiscordChannel before, SolaceDiscordChannel after) : base(before, after)
            {
                
            }
            
            public override string GetDifferenceString()
            {
                return Before.GetChannelDifference(After);
            }
        }
        
        public class GuildDifference : BaseDifference<SolaceDiscordGuild>
        {
            public GuildDifference(SolaceDiscordGuild before, SolaceDiscordGuild after) : base(before, after)
            {
                
            }
            
            public override string GetDifferenceString()
            {
                return Before.GetGuildDifference(After);
            }
        }
        
        public class GuildUserDifference : BaseDifference
        {
            public SolaceDiscordUser User { get; set; }
            public SolaceDiscordGuild Guild { get; set; }
            public string NicknameBefore { get; set; }
            public string NicknameAfter { get; set; }
            public IEnumerable<SolaceDiscordRole> RolesAdded { get; set; }
            public IEnumerable<SolaceDiscordRole> RolesRemoved { get; set; }
            
            public GuildUserDifference(
                SolaceDiscordUser user,
                SolaceDiscordGuild guild,
                string before_nick,
                string after_nick,
                IEnumerable<SolaceDiscordRole> roles_added,
                IEnumerable<SolaceDiscordRole> roles_removed
            )
            {
                User           = user;
                Guild          = guild;
                NicknameBefore = before_nick;
                NicknameAfter  = after_nick;
                RolesAdded     = roles_added;
                RolesRemoved   = roles_removed;
            }
            
            public override string GetDifferenceString()
            {
                var diff = new StringBuilder();
                if (NicknameBefore != NicknameAfter)
                {
                    var before = string.IsNullOrEmpty(NicknameBefore) ? "[None]" : NicknameBefore;
                    var after  = string.IsNullOrEmpty(NicknameAfter ) ? "[None]" : NicknameAfter;
                    diff.Append($"Nickname: {before} -> {after}");
                }
                
                var roles_added = false;
                if (RolesAdded.Count() > 0)
                {
                    roles_added = true;
                    if (diff.Length > 0)
                    {
                        diff.Append(". ");
                    }
                    diff.Append("Roles Added: ");
                    foreach (var role in RolesAdded)
                    {
                        diff.Append($"\"{role.Name}\" ({role.Id}), ");
                    }
                }
                var roles_removed = false;
                if (RolesRemoved.Count() > 0)
                {
                    roles_removed = true;
                    if (roles_added)
                    {
                        // remove trailing ", "
                        diff.Remove(diff.Length - 2, 2);
                    }
                    if (roles_added || diff.Length > 0)
                    {
                        diff.Append(". ");
                    }
                    diff.Append("Roles Removed: ");
                    foreach (var role in RolesRemoved)
                    {
                        diff.Append($"\"{role.Name}\" ({role.Id}), ");
                    }
                }
                
                if (roles_added || roles_removed)
                {
                    // remove trailing ", "
                    diff.Remove(diff.Length - 2, 2);
                }
                
                return diff.ToString();
            }
        }
    }
}
