
namespace Solace.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IService
    {
        /// <summary>
        /// Called once, the first time the service is ever
        /// run in an installed instance of Solace.
        /// </summary>
        /// <param name="config"></param>
        void Install(IConfiguration config);
        
        /// <summary>
        /// Called once when the user is requesting permanent removal
        /// of the installed service.
        /// </summary>
        void Uninstall();
        
        /// <summary>
        /// Called once at the start of every session of this service,
        /// either when the program starts up or if the service was
        /// killed and restarted.
        /// </summary>
        /// <param name="config"></param>
        void Setup(IConfiguration config);
    }
}
