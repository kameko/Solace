
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public abstract class BaseSubsystem : ISubsystem
    {
        public string Name { get; protected set; }
        public int MessageDenialOfServicePreventionCutoff { get; set; }
        private List<CommunicationToken> Communications { get; set; }
        
        public BaseSubsystem()
        {
            Name = string.Empty;
            Communications = new List<CommunicationToken>();
            MessageDenialOfServicePreventionCutoff = 30;
        }
        
        protected abstract Task Pulse(Message messages);
        
        public virtual Task Pulse()
        {
            return Task.Run(async () =>
            {
                var tokens = new List<CommunicationToken>(Communications);
                foreach (var token in tokens)
                {
                    if (token.Closed)
                    {
                        Communications.Remove(token);
                    }
                    
                    int dos_protect = MessageDenialOfServicePreventionCutoff;
                    while (token.Receive(out var message))
                    {
                        await Pulse(message);
                        
                        dos_protect--;
                        if (dos_protect < 0)
                        {
                            break;
                        }
                    }
                }
            });
        }
        
        public virtual Task AddCommunications(IEnumerable<CommunicationToken> tokens)
        {
            return Task.Run(() => 
            {
                foreach (var token in tokens)
                {
                    Communications.Add(token);
                }
            });
        }
        
        public virtual void Dispose()
        {
            var tokens = new List<CommunicationToken>(Communications);
            foreach (var token in tokens)
            {
                token.Dispose();
            }
        }
        
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(Dispose));
        }
    }
}
