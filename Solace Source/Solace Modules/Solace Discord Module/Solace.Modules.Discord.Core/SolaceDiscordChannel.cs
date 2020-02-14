
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
        public string MentionString { get; set; }
        public int PerUserRateLimit { get; set; }
        public int Position { get; set; }
        public string Topic { get; set; }
        public SolaceDiscordGuild Guild { get; set; }
        
        public SolaceDiscordChannel()
        {
            Name          = string.Empty;
            MentionString = string.Empty;
            Topic         = string.Empty;
            Guild         = new SolaceDiscordGuild();
        }
        
        public string GetChannelDifference(SolaceDiscordChannel other)
        {
            return ObjectExtensions.GetShallowObjectDifferencesAsString(this, other);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is SolaceDiscordChannel other)
            {
                return Name             == other.Name
                    && Id               == other.Id
                    && MentionString    == other.MentionString
                    && PerUserRateLimit == other.PerUserRateLimit
                    && Position         == other.Position
                    && Topic            == other.Topic
                    && Guild.Equals(other.Guild);
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return new
            {
                Name,
                Id,
                MentionString,
                PerUserRateLimit,
                Position,
                Topic,
                Guild
            }
            .GetHashCode();
        }
    }
}
