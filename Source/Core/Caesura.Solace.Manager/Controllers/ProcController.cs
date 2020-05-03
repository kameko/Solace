
namespace Caesura.Solace.Manager.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Foundation.ApiBoundaries;
    
    [ApiController]
    [Route("system/[controller]")]
    public class ProcController : ProcControllerBase<ProcController>
    {
        public ProcController(ILogger<ProcController> logger, IConfiguration configuration)
            : base(logger, configuration)
        {
            
        }
    }
}
