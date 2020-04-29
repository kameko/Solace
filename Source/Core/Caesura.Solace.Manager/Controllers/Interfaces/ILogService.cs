
namespace Caesura.Solace.Manager.Controllers.Interfaces
{
    using System;
    using Entities.Core.Manager;
    using Foundation.ApiBoundaries;
    
    public interface ILogService : IControllerSearchableService<ulong, LogElement, string>
    {
        
    }
}
