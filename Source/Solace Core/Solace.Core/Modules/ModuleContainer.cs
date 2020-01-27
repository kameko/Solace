
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ModuleContainer
    {
        public ModuleLoader? Loader { get; set; }
        public BaseModule? Module { get; set; }
        
        public ModuleContainer()
        {
            
        }
    }
}
