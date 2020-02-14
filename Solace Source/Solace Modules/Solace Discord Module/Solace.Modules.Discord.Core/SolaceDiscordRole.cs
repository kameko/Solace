
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core;
    
    public class SolaceDiscordRole
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        
        public SolaceDiscordRole()
        {
            Name  = string.Empty;
        }
        
        public string GetRoleDifference(SolaceDiscordRole other)
        {
            return ObjectExtensions.GetShallowObjectDifferencesAsString(this, other);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is SolaceDiscordRole other)
            {
                return Name == other.Name && Id == other.Id;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return new
            {
                Name,
                Id
            }
            .GetHashCode();
        }
    }
}
