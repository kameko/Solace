
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface ISubsystem : IDisposable, IAsyncDisposable
    {
        string Name { get; }
        
        // TODO: something to fire off initial messages, including accepting
        // initial messages from a config
        
        Task Pulse();
        Task AddCommunications(IEnumerable<CommunicationToken> tokens);
    }
}
