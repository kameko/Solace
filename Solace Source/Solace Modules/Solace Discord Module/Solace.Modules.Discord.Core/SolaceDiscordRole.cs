
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
        
        // TODO: override Equals
    }
}
