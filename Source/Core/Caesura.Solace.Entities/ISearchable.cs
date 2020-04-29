
namespace Caesura.Solace.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface ISearchable<T>
    {
        bool Search(string field, string term, int limit, out IEnumerable<T> result);
    }
}
