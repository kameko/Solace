
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Modules;
    using Subsystems;
    
    public class ModuleSubsystemCoordinator
    {
        private SubsystemManager Subsystems { get; set; }
        private ModuleManager Modules { get; set; }
        
        public ModuleSubsystemCoordinator()
        {
            Subsystems = new SubsystemManager();
            Modules    = new ModuleManager();
        }
    }
}
