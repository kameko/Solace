
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class SolaceDiscordVoiceState
    {
        public SolaceDiscordUser User { get; set; }
        public ulong GuildId { get; set; }
        public string GuildName { get; set; }
        public bool SelfDeafened { get; set; }
        public bool SelfMuted { get; set; }
        public bool GuildDeafened { get; set; }
        public bool GuildMuted { get; set; }
        public bool Suppressed { get; set; }
        
        public SolaceDiscordVoiceState()
        {
            User = new SolaceDiscordUser();
            GuildName = string.Empty;
        }
        
        public string GetDifferenceString(SolaceDiscordVoiceState other)
        {
            return GetDifferenceString(this, other);
        }
        
        public static string GetDifferenceString(SolaceDiscordVoiceState before, SolaceDiscordVoiceState after)
        {
            var diff = string.Empty;
            
            void AppendComma()
            {
                if (!string.IsNullOrEmpty(diff))
                {
                    diff += ", ";
                }
            }
            
            if (before.SelfDeafened != after.SelfDeafened)
            {
                diff += $"Self Defened: {before.SelfDeafened} -> {after.SelfDeafened}";
            }
            if (before.SelfMuted != after.SelfMuted)
            {
                AppendComma();
                diff += $"Self Muted: {before.SelfMuted} -> {after.SelfMuted}";
            }
            if (before.GuildDeafened != after.GuildDeafened)
            {
                AppendComma();
                diff += $"Guild Deafened: {before.GuildDeafened} -> {after.GuildDeafened}";
            }
            if (before.GuildMuted != after.GuildMuted)
            {
                AppendComma();
                diff += $"Guild Muted: {before.GuildMuted} -> {after.GuildMuted}";
            }
            if (before.Suppressed != after.Suppressed)
            {
                AppendComma();
                diff += $"Suppressed: {before.Suppressed} -> {after.Suppressed}";
            }
            
            return diff;
        }
    }
}
