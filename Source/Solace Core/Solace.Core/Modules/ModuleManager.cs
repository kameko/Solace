
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ModuleManager
    {
        private List<ModuleContainer> Containers { get; set; }
        
        public ModuleManager()
        {
            Containers = new List<ModuleContainer>();
        }
    }
}
