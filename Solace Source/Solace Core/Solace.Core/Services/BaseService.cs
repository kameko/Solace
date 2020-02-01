
namespace Solace.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public abstract class BaseService : IService
    {
        public virtual void Install(IConfiguration config)
        {
            throw new NotImplementedException();
        }
        
        public virtual void Uninstall()
        {
            throw new NotImplementedException();
        }
        
        public virtual void Setup(IConfiguration config)
        {
            throw new NotImplementedException();
        }
    }
}
