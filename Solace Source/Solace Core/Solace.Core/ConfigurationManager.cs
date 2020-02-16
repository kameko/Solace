
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ConfigurationManager
    {
        // TODO: just use JSON, needs to be programmable anyway.
        public string Location { get; set; }
        
        public ConfigurationManager(string location)
        {
            Location = location;
        }
        
        public IConfiguration Load()
        {
            return default!;
        }
    }
}
