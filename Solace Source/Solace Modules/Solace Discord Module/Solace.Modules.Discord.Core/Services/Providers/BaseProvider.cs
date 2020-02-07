
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core.Services;
    
    public abstract class BaseProvider : BaseService, IProvider
    {
        public BaseProvider() : base()
        {
            
        }
        
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}
