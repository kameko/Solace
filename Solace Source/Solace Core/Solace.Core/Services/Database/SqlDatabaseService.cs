
namespace Solace.Core.Services.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public abstract class SqlDatabaseService : DatabaseService, ISqlDatabaseService
    {
        public string ConnectionString { get; protected set; }
        
        public SqlDatabaseService()
        {
            ConnectionString = string.Empty;
        }
        
        public virtual Task Connect(string connection_string)
        {
            return Task.CompletedTask;
        }
    }
}
