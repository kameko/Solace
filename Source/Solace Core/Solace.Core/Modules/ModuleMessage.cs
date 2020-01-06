
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ModuleMessage
    {
        public ModuleInfo Sender { get; internal set; }
        public object? Data { get; set; }
        
        public ModuleMessage(ModuleInfo info, object data)
        {
            Sender = info;
            Data   = data;
        }
        
        public ModuleMessage(ModuleInfo info) : this(info, null!)
        {
            
        }
    }
}
