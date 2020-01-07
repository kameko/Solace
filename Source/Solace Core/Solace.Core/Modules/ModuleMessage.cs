
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ModuleMessage
    {
        public ModuleInfo Sender { get; internal set; }
        public string Event { get; set; }
        public object? Data { get; set; }
        
        public ModuleMessage(string name, ModuleInfo info, object data)
        {
            Sender = info;
            Event  = name;
            Data   = data;
        }
        
        public ModuleMessage(string name, ModuleInfo info) : this(name, info, null!)
        {
            
        }
    }
}
