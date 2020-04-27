
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
            
            using (var context = new LogElementContext())
            {
                var elms = context.LogItems.Take(100);
                var ok   = LogServiceResult.GetAll.Ok(elms);
                log.ExitMethod(nameof(Get));
                return Task.FromResult(ok);
            }
        }
        
        public Task<LogServiceResult.GetById> Get(ulong id)
        {
            log.EnterMethod(nameof(Get), "for Id {Id}.", id);
            
            using (var context = new LogElementContext())
            {
                var elm = context.LogItems.Find(id);
                if (elm is null)
                {
                    var bad = LogServiceResult.GetById.NotFound();
                    log.ExitMethod(nameof(Get));
                    return Task.FromResult(bad);
                }
                else
                {
                    var ok = LogServiceResult.GetById.Ok(elm);
                    log.ExitMethod(nameof(Get));
                    return Task.FromResult(ok);
                }
            }
        }
        
        public Task<LogServiceResult.Put> Put(ulong id, LogElement element)
        {
            log.EnterMethod(nameof(Put), "for Id {Id} and LogElement {LogElement}.", id, element);
            
            log.Debug("This method is not implemented.");
            
            log.ExitMethod(nameof(Put), "for Id {Id} and LogElement {LogElement}.", id, element);
            
            return Task.FromResult(LogServiceResult.Put.Unauthorized());
        }
        
        public async Task<LogServiceResult.Post> Post(LogElement element)
        {
            log.EnterMethod(nameof(Post), "for LogElement {LogElement}.", element);
            
            try
            {
                using (var context = new LogElementContext())
                {
                    var elm = context.LogItems.Find(element.Id);
                    if (elm is null)
                    {
                        context.Add(element);
                        await context.SaveChangesAsync();
                        elm = context.LogItems.Find(element.Id);
                        if (elm is null)
                        {
                            throw new Exception($"LogElement with Id {element.Id} was added but not found!");
                        }
                        else
                        {
                            return LogServiceResult.Post.Ok(elm);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"LogElement with Id {element.Id} already exists.");
                    }
                }
            }
            catch (Exception e)
            {
                return LogServiceResult.Post.Bad(e);
            }
            finally
            {
                log.ExitMethod(nameof(Post), "for LogElement {LogElement}.", element);
            }
        }
        
        public Task<LogServiceResult.Delete> Delete(ulong id)
        {
            log.EnterMethod(nameof(Delete), "for Id {Id}.", id);
            
            log.Debug("This method is not implemented.");
            
            log.ExitMethod(nameof(Delete), "for Id {Id}.", id);
            
            return Task.FromResult(LogServiceResult.Delete.Unauthorized());
        }
    }
}
