
namespace Caesura.Solace.Manager.Controllers.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Foundation.Logging;
    using Foundation.ApiBoundaries;
    using Foundation.ConfigurationModels;
    using Entities.Core.Manager;
    using Entities.Core.Manager.Contexts;
    using Interfaces;
    using ConfigurationModels;
    
    public class LogService 
        : EntityFrameworkControllerService<LogService, ulong, LogElement, string, LogElementContext>
        , ILogService
    {
        private LogElementContext le_context;
        
        public LogService(ILogger<LogService> ilog, IConfiguration configuration, LogElementContext lec)
            : base(ilog, configuration)
        {
            le_context = lec;
            
            var storage_model = Configuration.GetSection(ConfigurationConstants.Storage).Get<StorageModel>();
            SourcePath = new FileInfo(storage_model.Log.Path);
            
            var networking_model = Configuration.GetSection(ConfigurationConstants.Networking).Get<NetworkingModel>();
            GetLimit = networking_model.GetLimit;
        }
        
        // ---
        
        public override async Task<ControllerResult.GetAll<LogElement>> GetAll()
        {
            return await DefaultGetAll(context => context.LogElements.Take(GetLimit));
        }
        
        public override async Task<ControllerResult.GetById<LogElement>> GetById(ulong id)
        {
            return await DefaultGetById(id, context => context.LogElements.Find(id));
        }
        
        public override async Task<ControllerResult.Post<LogElement>> Post(LogElement value)
        {
            return await DefaultPost(context => context.LogElements.Find(value.Id), context => context.Add(value));
        }
        
        // ---
        
        protected override LogElementContext ContextFactory() => le_context;
        
        protected override void SeedFactory(LogElementContext context)
        {
            var le = new LogElement()
            {
                TimeStamp       = DateTime.UtcNow,
                Level           = LogLevel.Information,
                SenderService   = nameof(LogService),
                ReceiverService = nameof(LogService),
                Message         = "Created log database.",
            };
            context.LogElements.Add(le);
        }
    }
}
