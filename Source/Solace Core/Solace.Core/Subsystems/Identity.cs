
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class Identity
    {
        public string Name { get; private set; }
        
        public Identity(string name)
        {
            Name = name;
        }
    }
}
