
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IConfiguration
    {
        string GetValue(string key);
    }
}
