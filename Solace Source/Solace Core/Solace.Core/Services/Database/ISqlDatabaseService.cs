
namespace Solace.Core.Services.Database
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface ISqlDatabaseService : IDatabaseService
    {
        Task Connect(string connection_string);
    }
}
