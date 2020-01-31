
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    // TODO: hiarchy-based capabilities:
    // DATABASE/SQL/POSTGRES
    // CHAT/DISCORD
    // MICROBLOG/TWITTER
    // Each layer of a hiarchy has it's own set of APIs for
    // how it should be interacted with that expands with
    // each layer, with all layers having a guarenteed
    // Send and Receive Message API as all subsystems do.
    // This way a subsystem can choose to target a very broad
    // API, like if it just wants to save something without
    // caring if the database is even SQL or not, or go all
    // the way to the Postgres layer to do specific Postgres
    // maintenance.
    
    // Goal of Capabilities: To provide what is essentially
    // a runtime-constructed class that consumers do not need
    // to know about at compile/link time and do not need an
    // external dependency for. As they are constructed in a
    // phase or hiarchy fashion, subsystems can choose how
    // specific they require another subsystem to be when
    // implementing a capability.
    // Essentially what a Capability is, is it tells consumers
    // what messages a subsystem takes and how to construct them.
    
    public class Capability
    {
        public string Name { get; protected set; }
        
        public Capability()
        {
            Name = string.Empty;
        }
        
        
    }
}
