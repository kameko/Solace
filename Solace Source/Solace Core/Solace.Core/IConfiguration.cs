
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IConfiguration
    {
        T GetValue<T>();
        T GetValue<T>(string key);
        object GetValue(string key);
        bool TryGetValue<T>(out T item);
        bool TryGetValue<T>(string key, out T item);
        bool SetValue(string key, object value);
    }
}
