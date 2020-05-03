
namespace Caesura.Solace.Manager.ServiceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Foundation.ApiBoundaries.HttpClients.Core.Database;
    
    // TODO: Add to documentation that in order to add a new service
    // to the system, the user must create a HttpClient for it and
    // then add it to this class as a property and to the constructor
    // for the dependency injector.
    
    public class ServiceCollection
    {
        public DatabaseClient Database { get; private set; }
        
        public ServiceCollection(
            DatabaseClient database
        )
        {
            Database = database;
        }
    }
}
