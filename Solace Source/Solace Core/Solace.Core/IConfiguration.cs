
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IConfiguration
    {
        bool AddValue(string service, string key, object value);
    }
    
    public interface IConfiguration<T> : IConfiguration
    {
        Dictionary<string, T> Configuration { get; set; }
    }
}
