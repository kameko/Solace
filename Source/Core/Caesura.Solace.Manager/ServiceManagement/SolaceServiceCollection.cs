
namespace Caesura.Solace.Manager.ServiceManagement
{
    using System;
    using System.Collections.Generic;
    using Foundation.ApiBoundaries.HttpClients;
    using Foundation.ApiBoundaries.HttpClients.Core.Database;
    
    // TODO: Add to documentation that in order to add a new service
    // to the system, the user must create a HttpClient for it and
    // then add it to this class as a property and to the constructor
    // for the dependency injector.
    
    public interface ISolaceServiceCollection
    {
        DatabaseClient Database { get; }
        bool TryGet(string name, out IBaseClient? client);
    }
    
    public class SolaceServiceCollection : ISolaceServiceCollection
    {
        public DatabaseClient Database { get; private set; }
        
        public SolaceServiceCollection(
            DatabaseClient database
        )
        {
            Database = database;
            clients = new Dictionary<string, IBaseClient>()
            {
                { Database.Name, Database },
            };
        }
        
        private Dictionary<string, IBaseClient> clients;
        
        public IBaseClient this[string name]
        {
            get => clients[name];
        }
        
        public bool TryGet(string name, out IBaseClient? client)
        {
            var success = clients.TryGetValue(name, out client);
            return success;
        }
    }
}
