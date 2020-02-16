
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Services.Providers;
    using Solace.Core;
    
    public class DiscordConfig
    {
        public string ConnectionToken { get; set; }
        public bool DebugLog { get; set; }
        public int PingTimeout { get; set; }
        public int PingTries { get; set; }
        public Log.LogLevel LogLevel { get; set; }
        
        public DiscordConfig()
        {
            ConnectionToken = string.Empty;
            PingTimeout     = 5000;
            PingTries       = 3;
            LogLevel        = Log.LogLevel.Debug;
        }
    }
}
