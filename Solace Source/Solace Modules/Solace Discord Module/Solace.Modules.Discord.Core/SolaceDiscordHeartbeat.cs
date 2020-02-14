
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class SolaceDiscordHeartbeat
    {
        public DateTimeOffset Timestamp { get; set; }
        public int Ping { get; set; }
        public int IntegrityChecksum { get; set; }
        
        public SolaceDiscordHeartbeat()
        {
            
        }
    }
}
