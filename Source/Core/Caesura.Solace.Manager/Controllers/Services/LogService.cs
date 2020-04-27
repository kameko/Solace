
namespace Caesura.Solace.Manager.Controllers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Foundation.Logging;
    using Entities.Core;
    using Entities.Core.Contexts;
    using Interfaces;
    
    public class LogService : ILogService
    {
        private ILogger log;
        
        public LogService(ILogger<LogService> ilog)
        {
            log = ilog;
            
            log.InstanceAbreaction();
        }
        
        public Task<LogServiceResult.GetAll> Get()
        {
            log.EnterMethod(nameof(Get));
            
            // TODO: have this return the last 100 log elements.
            
            var items = new List<LogElement>()
            {
                new LogElement()
                {
                    Id = 1,
                    Message = "Hello!"
                },
                new LogElement()
                {
                    Id = 2,
                    Message = "Hello, again!"
                },
                new LogElement()
                {
                    Id = 3,
                    Message = "Hello, once more!"
                },
            };
            
            log.ExitMethod(nameof(Get));
            return Task.FromResult(LogServiceResult.GetAll.Ok(items));
            // throw new NotImplementedException();
        }
        
        public async Task<LogServiceResult.GetById> Get(ulong id)
        {
            log.EnterMethod(nameof(Get), "for Id {Id}.", id);
            
            var items = await Get();
            var item = (items.Value as List<LogElement>)!.Find(x => x.Id == id);
            
            LogServiceResult.GetById retval = null!;
            if (item is null)
            {
                retval = LogServiceResult.GetById.NotFound();
            }
            else
            {
                retval = LogServiceResult.GetById.Ok(item);
            }
            
            log.ExitMethod(nameof(Get), "for Id {Id}.", id);
            return retval;
            //throw new NotImplementedException();
        }
        
        public Task<LogServiceResult.Put> Put(ulong id, LogElement element)
        {
            log.EnterMethod(nameof(Put), "for Id {Id} and LogElement {LogElement}.", id, element);
            
            // ...
            
            log.ExitMethod(nameof(Put), "for Id {Id} and LogElement {LogElement}.", id, element);
            
            return Task.FromResult(LogServiceResult.Put.Unauthorized());
        }
        
        public Task<LogServiceResult.Post> Post(LogElement element)
        {
            log.EnterMethod(nameof(Post), "for LogElement {LogElement}.", element);
            
            // ...
            
            log.ExitMethod(nameof(Post), "for LogElement {LogElement}.", element);
            
            throw new NotImplementedException();
        }
        
        public Task<LogServiceResult.Delete> Delete(ulong id)
        {
            log.EnterMethod(nameof(Delete), "for Id {Id}.", id);
            
            // ...
            
            log.ExitMethod(nameof(Delete), "for Id {Id}.", id);
            
            return Task.FromResult(LogServiceResult.Delete.Unauthorized());
        }
    }
}
