
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ConfigurationElement
    {
        public Dictionary<string, string> Configuration { get; set; }
        
        public ConfigurationElement()
        {
            Configuration = new Dictionary<string, string>();
        }
        
        public bool ElementAs<T>(string key, out T item)
        {
            var success = Configuration.TryGetValue(key, out var value);
            if (success && value is T titem)
            {
                item = titem;
                return true;
            }
            item = default!;
            return false;
        }
    }
}
