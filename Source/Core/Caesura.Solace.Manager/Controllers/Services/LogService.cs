
namespace Caesura.Solace.Manager.Controllers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Entities.Core;
    using Interfaces;
    
    public class LogService : ILogService
    {
        private ILogger log;
        
        public LogService() // TODO: see if the DI can give this an ILogger directly
        {
            log = null!;
        }
        
        public void Inject(ILogger ilog)
        {
            log = ilog;
        }
        
        public Task<LogServiceResult.GetAll> Get()
        {
            throw new NotImplementedException();
        }
        
        public Task<LogServiceResult.GetById> Get(ulong id)
        {
            throw new NotImplementedException();
        }
        
        public Task<LogServiceResult.Put> Put(ulong id, LogElement element)
        {
            return Task.FromResult(LogServiceResult.Put.Unauthorized());
        }
        
        public Task<LogServiceResult.Post> Post(LogElement element)
        {
            throw new NotImplementedException();
        }
        
        public Task<LogServiceResult.Delete> Delete(ulong id)
        {
            return Task.FromResult(LogServiceResult.Delete.Unauthorized());
        }
    }
}
