
namespace Caesura.Solace.Manager.ConfigurationModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ServicesModel
    {
        public Dictionary<string, Service> Items { get; set; }
        
        public ServicesModel()
        {
            Items = new Dictionary<string, Service>();
        }
        
        public class Service
        {
            public string ExecPath { get; set; }
            public string Connection { get; set; }
            public bool Local { get; set; }
            
            public Service()
            {
                ExecPath   = string.Empty;
                Connection = string.Empty;
            }
        }
    }
}
