
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public abstract class BaseModule
    {
        public bool Unloadable { get; protected set; }
        public ModuleInfo Info { get; protected set; }
        public List<string> Dependencies { get; protected set; }
        
        public BaseModule()
        {
            Unloadable   = true;
            Info         = new ModuleInfo();
            Dependencies = new List<string>();
        }
    }
}
