
namespace Caesura.Solace.Manager.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Foundation.ApiBoundaries;
    using Entities.Core;
    using Entities.Core.Contexts;
    using Interfaces;
    
    [ApiController]
    [Route("system/[controller]")]
    public class LogController
        : BaseServiceController<LogController, LogElement, ILogService, LogElementContext>
    {
        public LogController(ILogger<LogController> ilog, IConfiguration configuration, ILogService iservice)
            : base(iservice, ilog, configuration)
        {
            
        }
        
        [HttpGet]
        public Task<ActionResult<IEnumerable<LogElement>>> Get() => GetAll();
        
        [HttpGet("search/{field}/{term}")]
        public Task<ActionResult<LogElement>> Get(string field, string term) => GetBySearch(field, term);
        
        [HttpGet("{id}")]
        public Task<ActionResult<LogElement>> Get(ulong id) => GetById(id);
        
        [HttpPost]
        public Task<ActionResult<LogElement>> Post(LogElement element) => PostDefault(element);
    }
}
