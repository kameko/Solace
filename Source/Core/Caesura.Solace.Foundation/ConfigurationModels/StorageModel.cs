
namespace Caesura.Solace.Foundation.ConfigurationModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class StorageModel
    {
        public LogModel Log { get; set; }
        
        public StorageModel()
        {
            Log = new LogModel();
        }
        
        public class LogModel
        {
            public string Path { get; set; }
            public string ConnectionString { get; set; }
            
            public LogModel()
            {
                Path             = string.Empty;
                ConnectionString = string.Empty;
            }
        }
    }
}
