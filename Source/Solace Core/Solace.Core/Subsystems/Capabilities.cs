
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
    public class Capabilities
    {
        
    }
}
