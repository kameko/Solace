
namespace Caesura.Solace.Manager.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNet.OData;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Foundation.ApiBoundaries;
    using Foundation.Logging;
    using Entities.Core.Manager;
    using Interfaces;
    
    [ApiController]
    [Route("system/[controller]")]
    public class LogController
        : BaseServiceController<LogController, LogElement, ILogService>
    {
        public LogController(ILogger<LogController> ilog, IConfiguration configuration, ILogService iservice)
            : base(iservice, ilog, configuration)
        {
            Log.InstanceAbreaction();
        }
        
        [HttpGet]
        [EnableQuery]
        public Task<ActionResult<IEnumerable<LogElement>>> Get() => GetAllDefault();
        
        [HttpGet("{id}")]
        public Task<ActionResult<LogElement>> Get(ulong id) => GetByIdDefault(id);
        
        [HttpPost]
        public Task<ActionResult<LogElement>> Post(LogElement element) => PostDefault(element);
    }
}
