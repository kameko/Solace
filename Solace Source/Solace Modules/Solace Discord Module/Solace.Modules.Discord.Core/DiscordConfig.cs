
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
        public DateTime DeleteMessagesFromDatabaseOlderThan { get; set; }
        public int PingTimeout { get; set; }
        public int PingTries { get; set; }
        public bool DebugLog { get; set; }
        public Log.LogLevel LogLevel { get; set; }
        
        public DiscordConfig()
        {
            ConnectionToken = string.Empty;
            DeleteMessagesFromDatabaseOlderThan = new DateTime(2009, 1, 1);
            PingTimeout     = 5000;
            PingTries       = 3;
            DebugLog        = true;
            LogLevel        = Log.LogLevel.Debug;
        }
        
        public void FromConfig(ConfigurationToken config)
        {
            
        }
    }
}
