
namespace Caesura.Solace.Manager.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Foundation.ApiBoundaries;
    
    [ApiController]
    [Route("system/[controller]")]
    public class PidController : PidGetControllerBase<PidController>
    {
        public PidController(ILogger<PidController> logger) : base(logger)
        {
            
        }
    }
}
