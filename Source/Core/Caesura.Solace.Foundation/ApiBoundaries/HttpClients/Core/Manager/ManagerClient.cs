
namespace Caesura.Solace.Foundation.ApiBoundaries.HttpClients.Core.Manager
{
    using System;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    
    public class ManagerClient : BaseClient<ManagerClient>
    {
        public static string ConfigName => "Core.Manager";
        
        public ManagerClient(ILogger<ManagerClient> logger, IConfiguration configuration, HttpClient client)
            : base(ConfigName, logger, configuration, client)
        {
            
        }
        
        // TODO: add GET/POST for log entries.
    }
}
