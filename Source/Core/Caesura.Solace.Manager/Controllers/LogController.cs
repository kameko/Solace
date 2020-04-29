
namespace Caesura.Solace.Manager.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Foundation.Logging;
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
    }
}
