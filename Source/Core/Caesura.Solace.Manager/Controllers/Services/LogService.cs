
namespace Caesura.Solace.Manager.Controllers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Foundation.Logging;
    using Entities.Core;
    using Entities.Core.Contexts;
    using Interfaces;
    
    public class LogService : ILogService
    {
        private FileInfo db_path;
        private string db_connection;
        private int get_limit;
        private readonly ILogger log;
        private readonly IConfiguration config;
        
        public LogService(ILogger<LogService> ilog, IConfiguration configuration)
        {
            log    = ilog;
            config = configuration;
            
            db_path       = null!;
            db_connection = null!;
            Reconfigure();
            
            log.InstanceAbreaction();
        }
        
        private void Reconfigure()
        {
            db_path       = new FileInfo(config["LogService:DatabasePath"]);
            db_connection = config["LogService:ConnectionString"].Replace("{DatabasePath}", db_path.FullName);
            if (!int.TryParse(config["LogService:GetLimit"], out get_limit))
            {
                get_limit = 100;
            }
        }
        
        public async Task<LogServiceResult.GetAll> Get()
        {
            log.EnterMethod(nameof(Get));
            
            await CreateDatabaseIfNotExist();
            
            var elms = new List<LogElement>(get_limit);
            using (var context = new LogElementContext(db_connection))
            {
                var db_elms = context.LogElements.Take(get_limit);
                // required or the context will throw when we try to pass
                // db_elms to the caller.
                elms.AddRange(db_elms);
                var ok = LogServiceResult.GetAll.Ok(elms);
                log.ExitMethod(nameof(Get));
                return ok;
            }
        }
        
        public async Task<LogServiceResult.GetById> Get(ulong id)
        {
            log.EnterMethod(nameof(Get), "for Id {Id}.", id);
            
            await CreateDatabaseIfNotExist();
            
            using (var context = new LogElementContext(db_connection))
            {
                var elm = context.LogElements.Find(id);
                if (elm is null)
                {
                    var bad = LogServiceResult.GetById.NotFound();
                    log.ExitMethod(nameof(Get));
                    return bad;
                }
                else
                {
                    var ok = LogServiceResult.GetById.Ok(elm);
                    log.ExitMethod(nameof(Get));
                    return ok;
                }
            }
        }
        
        public Task<LogServiceResult.GetBySearch> Get(string field, string term)
        {
            log.EnterMethod(nameof(Get), "with search field {field} and term {term}", field, term);
            
            var elms = new List<LogElement>(get_limit);
            using (var context = new LogElementContext(db_connection))
            {
                if (field == "name")
                {
                    var db_elms = context.LogElements.Where(x => x.Name == term);
                    elms.AddRange(db_elms);
                }
                else if (field == "message")
                {
                    var newterm = term.ToLower();
                    var db_elms = context.LogElements.Where(x => 
                        x.Message.ToLower().Contains(newterm));
                    elms.AddRange(db_elms);
                }
                else if (field == "before")
                {
                    var dt_success = DateTime.TryParse(term, out var dt);
                    if (dt_success)
                    {
                        var db_elms = context.LogElements.Where(x => x.TimeStamp < dt).Take(get_limit);
                        elms.AddRange(db_elms);
                    }
                    else
                    {
                        return Task.FromResult(LogServiceResult.GetBySearch.InvalidTerm(term));
                    }
                }
                else if (field == "after")
                {
                    var dt_success = DateTime.TryParse(term, out var dt);
                    if (dt_success)
                    {
                        var db_elms = context.LogElements.Where(x => x.TimeStamp > dt).Take(get_limit);
                        elms.AddRange(db_elms);
                    }
                    else
                    {
                        return Task.FromResult(LogServiceResult.GetBySearch.InvalidTerm(term));
                    }
                }
                else if (field == "exception-name")
                {
                    var db_elms = context.LogElements.Where(x => x.Exception.Name == term);
                    elms.AddRange(db_elms);
                }
                else if (field == "exception-message")
                {
                    var newterm = term.ToLower();
                    var db_elms = context.LogElements.Where(x => 
                        x.Exception.Message.ToLower().Contains(newterm));
                    elms.AddRange(db_elms);
                }
                else
                {
                    return Task.FromResult(LogServiceResult.GetBySearch.InvalidField(field));
                }
            }
            log.ExitMethod(nameof(Get), "with search field {field} and term {term}", field, term);
            return Task.FromResult(LogServiceResult.GetBySearch.Ok(elms));
        }
        
        public Task<LogServiceResult.Put> Put(ulong id, LogElement element)
        {
            log.EnterMethod(nameof(Put), "for Id {Id} and LogElement {LogElement}.", id, element);
            
            log.Warning($"Method {nameof(Put)} is not meant to be implemented for this service.");
            
            log.ExitMethod(nameof(Put), "for Id {Id} and LogElement {LogElement}.", id, element);
            
            return Task.FromResult(LogServiceResult.Put.Unauthorized());
        }
        
        public async Task<LogServiceResult.Post> Post(LogElement element)
        {
            log.EnterMethod(nameof(Post), "for LogElement {LogElement}.", element);
            
            await CreateDatabaseIfNotExist();
            
            try
            {
                using (var context = new LogElementContext(db_connection))
                {
                    var elm = context.LogElements.Find(element.Id);
                    if (elm is null)
                    {
                        context.Add(element);
                        await context.SaveChangesAsync();
                        elm = context.LogElements.Find(element.Id);
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
            
            log.Warning($"Method {nameof(Delete)} is not meant to be implemented for this service.");
            
            log.ExitMethod(nameof(Delete), "for Id {Id}.", id);
            
            return Task.FromResult(LogServiceResult.Delete.Unauthorized());
        }
        
        private async Task CreateDatabaseIfNotExist()
        {
            if (!File.Exists(db_path.FullName))
            {
                log.Information($"Database file not found at {db_path.FullName}. Creating...");
                
                log.EnterMethod(nameof(CreateDatabaseIfNotExist));
                
                var le = new LogElement()
                {
                    TimeStamp = DateTime.UtcNow,
                    Level     = LogLevel.Information,
                    Name      = nameof(LogService),
                    Message   = "Created log database.",
                };
                
                var dir = db_path.Directory?.FullName ?? string.Empty;
                if (string.IsNullOrEmpty(dir))
                {
                    throw new ArgumentException($"{db_path.FullName} is not a valid path.");
                }
                else if (!Directory.Exists(dir))
                {
                    log.Information($"Database directory \"{dir}\" not found. Creating...");
                    Directory.CreateDirectory(dir);
                }
                
                using (var context = new LogElementContext(db_connection))
                {
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                    context.LogElements.Add(le);
                    await context.SaveChangesAsync();
                }
                
                log.Information($"Created system log database at {db_path.FullName}.");
                
                log.ExitMethod(nameof(CreateDatabaseIfNotExist));
            }
        }
    }
}
