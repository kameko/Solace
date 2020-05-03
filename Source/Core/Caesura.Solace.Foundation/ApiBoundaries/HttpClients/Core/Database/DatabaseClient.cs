
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
    
    public class DatabaseClient
    {
        private readonly ILogger Log;
        private readonly IConfiguration Configuration;
        public HttpClient Client { get; }
        
        public DatabaseClient(ILogger<DatabaseClient> logger, IConfiguration configuration, HttpClient client)
        {
            Log           = logger;
            Configuration = configuration;
            Client        = client;
            
            Client.BaseAddress = new Uri(Configuration["HttpClients:Core.Database:Connection"]);
        }
        
        public Task<int> RequestPid() => RequestPid((new CancellationTokenSource(5_000).Token));
        public Task<int> RequestPid(CancellationToken token) => ProcGetControllerBase.RequestPid(Client, token);
    }
}
