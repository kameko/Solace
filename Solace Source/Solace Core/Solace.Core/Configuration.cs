
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class Configuration : IConfiguration
    {
        public string Service { get; set; }
        private Dictionary<string, object> Values { get; set; }
        
        public Configuration()
        {
            Service = "SYSTEM";
            Values  = new Dictionary<string, object>();
        }
        
        public Configuration(string name) : this()
        {
            Service = name;
        }
        
        public T GetValue<T>()
        {
            var success = TryGetValue<T>(out var item);
            if (success)
            {
                return item;
            }
            else
            {
                throw new KeyNotFoundException($"Type of {typeof(T)}");
            }
        }
        
        public T GetValue<T>(string key)
        {
            var success = TryGetValue<T>(key, out var item);
            if (success)
            {
                return item;
            }
            else
            {
                throw new KeyNotFoundException($"Key {key}. Type of {typeof(T)}");
            }
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
        
        public bool SetValue(string key, object value)
        {
            if (value is null)
            {
                Values.Remove(key);
                return true;
            }
            else if (!Values.ContainsKey(key))
            {
                Values.Add(key, value);
                return true;
            }
            else
            {
                return false;
            }
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
