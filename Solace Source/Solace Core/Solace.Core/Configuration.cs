
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class Configuration : IConfiguration
    {
        public string Service { get; set; }
        public Dictionary<string, object> Values { get; set; }
        
        public Configuration(string name)
        {
            Service = name;
            Values  = new Dictionary<string, object>();
        }
        
        public Configuration() : this("[NONE]")
        {
            
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
                throw new KeyNotFoundException($"Type of \"{typeof(T)}\"");
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
                throw new KeyNotFoundException($"Key \"{key}\". Type of \"{typeof(T)}\"");
            }
        }
        
        public object GetValue(string key)
        {
            return Values[key];
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
        
        public bool SetValue(string key, object? value)
        {
            if (value is null)
            {
                Values.Remove(key);
                return true;
            }
            else if (Values.ContainsKey(key))
            {
                Values[key] = value;
                return true;
            }
            else
            {
                Values.Add(key, value);
                return true;
            }
        }
        
        public static Configuration GetDefault()
        {
            var cfg = new Configuration("System")
            {
                
            };
            return cfg;
        }
    }
}
