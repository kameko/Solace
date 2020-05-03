
namespace Caesura.Solace.Foundation.ApiBoundaries.HttpClients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    
    public abstract class BaseClient
    {
        public string Name { get; }
        protected ILogger Log { get; }
        protected IConfiguration Configuration { get; }
        public HttpClient Client { get; }
        public int TimeoutMs { get; protected set; }
        
        internal BaseClient(string name, ILogger logger, IConfiguration configuration, HttpClient client)
        {
            Name          = name;
            Log           = logger;
            Configuration = configuration;
            Client        = client;
            TimeoutMs     = 3_000;
            
            Client.BaseAddress = new Uri(Configuration[$"Services:Items:{Name}:Connection"]);
            if (int.TryParse(Configuration[$"Services:Items:{Name}:TimeoutMs"], out var timeout))
            {
                TimeoutMs = timeout;
            }
        }
        
        public virtual Task<int> RequestPid() => RequestPid((new CancellationTokenSource(5_000).Token));
        public virtual Task<int> RequestPid(CancellationToken token) => ProcGetControllerBase.RequestPid(Client, token);
    }
    
    public abstract class BaseClient<T> : BaseClient
    {
        public BaseClient(string name, ILogger<T> logger, IConfiguration configuration, HttpClient client)
            : base(name, logger, configuration, client)
        {
            
        }
    }
}
