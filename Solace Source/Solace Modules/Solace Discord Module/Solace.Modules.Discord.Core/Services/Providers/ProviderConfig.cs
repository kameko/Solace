
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
        public int PingTimeoutMilliseconds { get; set; }
        public int PingTries { get; set; }
        
        public ProviderConfig()
        {
            ConnectionToken = string.Empty;
            PingTimeoutMilliseconds = 5000;
            PingTries = 3;
        }
        
        public string? GetValue(string key)
        {
            return null;
        }
    }
}
