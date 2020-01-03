
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public abstract class BaseModule
    {
        public abstract ModuleInfo Info { get; protected set; }
        
        public virtual void OnLoad()
        {
            
        }
        
        public virtual void OnUnload()
        {
            
        }
        
        public virtual void OnReload()
        {
            
        }
        
        public virtual void OnModuleLoad(ModuleInfo info)
        {
            
        }
        
        public virtual void OnModuleUnload(ModuleInfo info)
        {
            
        }
    }
}
