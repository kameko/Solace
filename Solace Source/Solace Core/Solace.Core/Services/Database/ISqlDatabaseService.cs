
namespace Solace.Core.Services.Database
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface ISqlDatabaseService : IDatabaseService
    {
        string ConnectionString { get; }
        Task Connect(string connection_string);
    }
}
