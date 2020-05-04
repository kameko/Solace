
namespace Caesura.Solace.Foundation.ConfigurationModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class NetworkingModel
    {
        public int GetLimit { get; set; }
        
        public NetworkingModel()
        {
            GetLimit = 100;
        }
    }
}
