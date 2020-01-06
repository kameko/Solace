
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ModuleInit
    {
        public Events Events { get; set; }
        
        public ModuleInit(Events events)
        {
            Events = events;
        }
    }
}
