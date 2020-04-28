
namespace Caesura.Solace.Manager.Controllers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities.Core;
    using Entities.Core.Contexts;
    using Foundation.ApiBoundaries;
    
    public interface ILogService : IControllerSearchableService<ulong, LogElement, string, LogElementContext>
    {
        
    }
}
