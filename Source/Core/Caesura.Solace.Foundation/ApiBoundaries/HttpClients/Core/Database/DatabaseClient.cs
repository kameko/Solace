
namespace Caesura.Solace.Foundation.ApiBoundaries.HttpClients.Core.Database
{
    using System;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    
    public class DatabaseClient : BaseClient<DatabaseClient>
    {
        public static string ConfigName => "Core.Database";
        
        public DatabaseClient(ILogger<DatabaseClient> logger, IConfiguration configuration, HttpClient client)
            : base(ConfigName, logger, configuration, client)
        {
            
        }
    }
}
