
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core;
    
    public class ProviderConfig : IConfiguration
    {
        public string ConnectionToken { get; set; }
        public bool DebugLog { get; set; }
        
        public ProviderConfig()
        {
            ConnectionToken = string.Empty;
        }
        
        public string? GetValue(string key)
        {
            return null;
        }
    }
}
