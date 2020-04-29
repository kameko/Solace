
namespace Caesura.Solace.Manager.Controllers.Interfaces
{
    using System;
    using Entities.Core;
    using Foundation.ApiBoundaries;
    
    public interface ILogService : IControllerSearchableService<ulong, LogElement, string>
    {
        
    }
}
