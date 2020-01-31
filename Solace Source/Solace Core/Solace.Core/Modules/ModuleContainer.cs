
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ModuleContainer
    {
        private ModuleLoader? Loader { get; set; }
        private BaseModule? Module { get; set; }
        
        public ModuleContainer()
        {
            
        }
        
        public BaseModule Load(string name, string path)
        {
            Loader = new ModuleLoader(name);
            return Loader.LoadModule(path);
        }
        
        public bool TryLoad(string name, string path, out BaseModule? module)
        {
            Loader = new ModuleLoader(name);
            return Loader.TryLoadModule(path, out module);
        }
        
        
    }
}
