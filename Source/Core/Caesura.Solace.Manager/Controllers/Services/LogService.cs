
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
        
        // TODO: other methods
        
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
