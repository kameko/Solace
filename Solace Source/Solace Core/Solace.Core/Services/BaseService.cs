
namespace Solace.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public abstract class BaseService : IService
    {
        public virtual void Install(IConfiguration config)
        {
            // No implementation.
        }
        
        public virtual void Uninstall()
        {
            // No implementation.
        }
        
        public virtual void Setup(IConfiguration config)
        {
            // No implementation.
        }
        
        public virtual void Start(CancellationToken token)
        {
            // No implementation.
        }
    }
}
