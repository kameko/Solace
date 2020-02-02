
namespace Solace.Core.Services.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    
    public abstract class SqlDatabaseService : DatabaseService, ISqlDatabaseService
    {
        public virtual Task Connect(string connection_string)
        {
            return Task.CompletedTask;
        }
    }
}
