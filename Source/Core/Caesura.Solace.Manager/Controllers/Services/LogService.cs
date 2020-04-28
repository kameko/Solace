
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
    using Foundation.ApiBoundaries;
    using Entities.Core;
    using Entities.Core.Contexts;
    using Interfaces;
    
    public class LogService 
        : EntityFrameworkControllerService<LogService, ulong, LogElement, string, LogElementContext>
        , ILogService
    {
        public LogService(ILogger<LogService> ilog, IConfiguration configuration) : base(ilog, configuration)
        {
            Reconfigure(nameof(LogService));
            
            Log.InstanceAbreaction();
        }
        
        public override async Task<ControllerResult.GetAll<LogElement>> GetAll()
        {
            return await DefaultGetAll(
                context => context.LogElements.Take(GetLimit),
                SourcePath,
                ContextFactory,
                Seeder
            );
        }
        
        public override async Task<ControllerResult.GetById<LogElement>> GetById(ulong id)
        {
            return await DefaultGetById(
                id,
                context => context.LogElements.Find(id),
                SourcePath,
                ContextFactory,
                Seeder
            );
        }
        
        public override async Task<ControllerResult.GetBySearch<LogElement>> GetBySearch(string field, string term)
        {
            Log.EnterMethod(nameof(GetBySearch), "with search field {field} and term {term}", field, term);
            
            await CreateDatabaseIfNotExist(SourcePath, ContextFactory, Seeder);
            
            var elms = new List<LogElement>(GetLimit);
            using (var context = new LogElementContext(SourceConnectionString))
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
                        var db_elms = context.LogElements.Where(x => x.TimeStamp < dt).Take(GetLimit);
                        elms.AddRange(db_elms);
                    }
                    else
                    {
                        return ControllerResult.GetBySearch<LogElement>.BadRequest(term);
                    }
                }
                else if (field == "after")
                {
                    var dt_success = DateTime.TryParse(term, out var dt);
                    if (dt_success)
                    {
                        var db_elms = context.LogElements.Where(x => x.TimeStamp > dt).Take(GetLimit);
                        elms.AddRange(db_elms);
                    }
                    else
                    {
                        return ControllerResult.GetBySearch<LogElement>.BadRequest(term);
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
                    return ControllerResult.GetBySearch<LogElement>.BadRequest(field);
                }
            }
            Log.ExitMethod(nameof(GetBySearch), "with search field {field} and term {term}", field, term);
            return ControllerResult.GetBySearch<LogElement>.Ok(elms);
        }
        
        public override async Task<ControllerResult.Post<LogElement>> Post(LogElement value)
        {
            return await DefaultPost(
                context => context.LogElements.Find(value.Id),
                context => context.Add(value),
                SourcePath,
                ContextFactory,
                Seeder
            );
        }
        
        private LogElementContext ContextFactory() => new LogElementContext(SourceConnectionString);
        
        private void Seeder(LogElementContext context)
        {
            var le = new LogElement()
            {
                TimeStamp = DateTime.UtcNow,
                Level     = LogLevel.Information,
                Name      = nameof(LogService),
                Message   = "Created log database.",
            };
            context.LogElements.Add(le);
        }
    }
}
