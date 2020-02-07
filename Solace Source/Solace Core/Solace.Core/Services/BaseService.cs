
namespace Solace.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public abstract class BaseService : IService
    {
        public string Name { get; protected set; }
        
        public BaseService()
        {
            Name = string.Empty;
        }
        
        public virtual Task Install(IConfiguration config)
        {
            // No implementation.
            return Task.CompletedTask;
        }
        
        public virtual Task Uninstall()
        {
            // No implementation.
            return Task.CompletedTask;
        }
        
        public virtual Task Setup(IConfiguration config, ServiceProvider services)
        {
            // No implementation.
            return Task.CompletedTask;
        }
        
        public virtual Task Start(CancellationToken token)
        {
            // No implementation.
            return Task.CompletedTask;
        }
    }
}
