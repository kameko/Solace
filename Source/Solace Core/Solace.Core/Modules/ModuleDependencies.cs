
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ModuleDependencies
    {
        private List<ModuleDependency> Dependencies { get; set; }
        
        public ModuleDependencies()
        {
            Dependencies = new List<ModuleDependency>();
        }
        
        public void AddDependency(ModuleInfo info)
        {
            var dep = new ModuleDependency(info);
            Dependencies.Add(dep);
        }
        
        public bool Validate()
        {
            // TODO: check for recursive dependencies and stuff
            return true;
        }
        
        internal class ModuleDependency
        {
            public ModuleInfo ModuleInfo { get; set; }
            
            public ModuleDependency(ModuleInfo info)
            {
                ModuleInfo = info;
            }
        }
    }
}
