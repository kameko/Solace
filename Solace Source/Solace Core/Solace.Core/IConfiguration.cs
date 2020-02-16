
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IConfiguration
    {
        T GetValue<T>();
        T GetValue<T>(string key);
        bool TryGetValue<T>(out T item);
        bool TryGetValue<T>(string key, out T item);
    }
}
