
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Subsystems;
    
    public abstract class BaseModule
    {
        public abstract ModuleInfo Info { get; protected set; }
        
        public BaseModule()
        {
            
        }
    }
}
