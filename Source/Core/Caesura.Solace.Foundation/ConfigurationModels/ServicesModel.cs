
namespace Caesura.Solace.Foundation.ConfigurationModels
{
    using System;
    using System.Collections.Generic;
    
    public class ServicesModel
    {
        public int ReconnectDelayMs { get; set; }
        public Dictionary<string, Service> Items { get; set; }
        
        public ServicesModel()
        {
            ReconnectDelayMs = 5_000;
            Items            = new Dictionary<string, Service>();
        }
        
        public class Service
        {
            public string Connection { get; set; }
            public int TimeoutMs { get; set; }
            
            public Service()
            {
                Connection   = string.Empty;
                TimeoutMs    = 3_000;
            }
        }
    }
}
