
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ConfigurationToken : IConfiguration<ConfigurationElement>
    {
        public Dictionary<string, ConfigurationElement> Configuration { get; set; }
        
        public ConfigurationToken()
        {
            Configuration = new Dictionary<string, ConfigurationElement>();
        }
        
        public bool AddValue(string service, string key, object value)
        {
            var success = Configuration.TryGetValue(service, out var conf);
            if (success)
            {
                conf!.Configuration.Add(key, value.ToString()!);
                return true;
            }
            return false;
        }
        
        public static ConfigurationToken GetDefault()
        {
            var cfg = new ConfigurationToken()
            {
                
            };
            return cfg;
        }
    }
}
