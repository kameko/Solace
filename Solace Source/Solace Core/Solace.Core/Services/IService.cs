
namespace Solace.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    
    public interface IService
    {
        string Name { get; }
        bool Ready { get; }
        
        IEnumerable<string> GetAllRequiredConfigurationTokens();
        
        /// <summary>
        /// Called once, the first time the service is ever
        /// run in an installed instance of Solace.
        /// </summary>
        /// <param name="config"></param>
        Task Install(ConfigurationManager config);
        
        /// <summary>
        /// Called once when the user is requesting permanent removal
        /// of the installed service.
        /// </summary>
        Task Uninstall();
        
        /// <summary>
        /// Called once at the start of every session of this service,
        /// either when the program starts up or if the service was
        /// killed and restarted.
        /// </summary>
        /// <param name="config"></param>
        Task Setup(ConfigurationManager config, ServiceProvider services);
        
        /// <summary>
        /// Start the service, if applicable.
        /// </summary>
        /// <param name="token"></param>
        Task Start(CancellationToken token);
    }
}
