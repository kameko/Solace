
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class SystemModule : BaseModule
    {
        public override ModuleInfo Info { get; protected set; }
        
        public SystemModule()
        {
            Info = new ModuleInfo()
            {
                Name = "SYSTEM",
                Company = "Caesura Software Solutions",
                Version = new Version(1, 0, 0, 0),
                
                Copyright = "Copyright 2020 Caesura Software Solutions"
            };
        }
        
        protected override void Setup(ModuleInit init)
        {
            
        }
    }
}
