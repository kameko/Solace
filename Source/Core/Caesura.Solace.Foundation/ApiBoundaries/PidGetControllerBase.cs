
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Logging;
    
    public abstract class PidGetControllerBase<T>
    {
        protected ILogger Log { get; }
        
        public PidGetControllerBase(ILogger<T> logger)
        {
            Log = logger;
        }
        
        [HttpGet]
        public int GetPid()
        {
            var pid = Process.GetCurrentProcess().Id;
            Log.Information("GET request for current Process ID. Returning {pid}.", pid);
            return pid;
        }
    }
}
