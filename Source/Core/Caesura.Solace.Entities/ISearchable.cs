
namespace Caesura.Solace.Entities
{
    using System;
    using System.Collections.Generic;
    
    // TODO: wouldn't this be better as a query string?
    // eg /search?name="service"&message-contains="stuff"
    // https://docs.microsoft.com/en-us/odata/webapi/first-odata-api 
    
    public interface ISearchable<T>
    {
        bool Search(string field, string term, int limit, out IEnumerable<T> result);
    }
}
