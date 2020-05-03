
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Logging;
    
    public abstract class ProcControllerBase
    {
        protected ILogger Log { get; }
        
        internal ProcControllerBase(ILogger logger)
        {
            Log = logger;
        }
        
        [HttpGet("pid")]
        public int GetPid()
        {
            var pid = Process.GetCurrentProcess().Id;
            Log.Information("GET request for current Process ID. Returning {pid}.", pid);
            return pid;
        }
        
        // TODO: POST a request to end the process here.
        
        public static async Task<int> RequestPid(HttpClient client, CancellationToken token)
        {
            try
            {
                var response = await client.GetAsync("/proc/pid", token);

                response.EnsureSuccessStatusCode();

                var responseStr = await response.Content!.ReadAsStringAsync();
                if (int.TryParse(responseStr, out int pid))
                {
                    return pid;
                }
                return -1;
            }
            catch (HttpRequestException)
            {
                return -1;
            }
        }
    }
    
    public abstract class ProcGetControllerBase<T> : ProcControllerBase
    {
        public ProcGetControllerBase(ILogger<T> logger) : base(logger)
        {
            
        }
    }
}
