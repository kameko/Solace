
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public abstract class BaseSubsystem : ISubsystem
    {
        public string Name { get; protected set; }
        private List<CommunicationToken> Communications { get; set; }
        
        public BaseSubsystem()
        {
            Name = string.Empty;
            Communications = new List<CommunicationToken>();
        }
        
        public virtual Task Pulse()
        {
            throw new NotImplementedException();
        }
        
        public virtual Task AddCommunications(IEnumerable<CommunicationToken> tokens)
        {
            throw new NotImplementedException();
        }
        
        public virtual void Dispose()
        {
            
        }
        
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(Dispose));
        }
    }
}
