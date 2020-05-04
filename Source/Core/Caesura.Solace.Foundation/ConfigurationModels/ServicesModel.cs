
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
            public string ExecPath { get; set; }
            public string Connection { get; set; }
            public int TimeoutMs { get; set; }
            public bool Local { get; set; }
            public bool Autostart { get; set; }
            public bool Autoclose { get; set; }
            public bool CreateWindow { get; set; }
            
            public Service()
            {
                ExecPath     = string.Empty;
                Connection   = string.Empty;
                TimeoutMs    = 3_000;
                Local        = true;
                Autostart    = false;
                Autoclose    = false;
                CreateWindow = true;
            }
        }
    }
}
