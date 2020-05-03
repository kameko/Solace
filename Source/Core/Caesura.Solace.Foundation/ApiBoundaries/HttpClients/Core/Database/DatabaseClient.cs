
namespace Caesura.Solace.Foundation.ApiBoundaries.HttpClients.Core.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    
    public class DatabaseClient : BaseClient<DatabaseClient>
    {
        public DatabaseClient(ILogger<DatabaseClient> logger, IConfiguration configuration, HttpClient client)
            : base("Core.Database", logger, configuration, client)
        {
            
        }
    }
}
