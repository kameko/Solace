
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class Configuration : IConfiguration
    {
        private Dictionary<string, object> Values { get; set; }
        
        public Configuration()
        {
            Values = new Dictionary<string, object>();
        }
        
        public T GetValue<T>()
        {
            var _ = TryGetValue<T>(out var item);
            return item;
        }
        
        public T GetValue<T>(string key)
        {
            var _ = TryGetValue<T>(key, out var item);
            return item;
        }
        
        public bool TryGetValue<T>(out T item)
        {
            foreach (var kvp in Values)
            {
                if (kvp.Value is T tk)
                {
                    item = tk;
                    return true;
                }
            }
            item = default!;
            return false;
        }
        
        public bool TryGetValue<T>(string key, out T item)
        {
            var success = Values.TryGetValue(key, out object? raw);
            if (success && raw is T tv)
            {
                item = tv;
                return true;
            }
            item = default!;
            return false;
        }
        
        public static Configuration GetDefault()
        {
            var cfg = new Configuration()
            {
                
            };
            return cfg;
        }
    }
}
