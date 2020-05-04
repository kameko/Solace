
namespace Caesura.Solace.Foundation.ConfigurationModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ProcModel
    {
        public bool AllowShutdown { get; set; }
        public int ShutdownDelayMs { get; set; }
        
        public ProcModel()
        {
            AllowShutdown   = true;
            ShutdownDelayMs = 3_000;
        }
    }
}
