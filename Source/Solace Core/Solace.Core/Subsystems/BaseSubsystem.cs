
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
        
        protected abstract Task Pulse(IEnumerable<Message> messages);
        
        public virtual Task Pulse()
        {
            return Task.Run(async () =>
            {
                var messages = new List<Message>();
                var tokens   = new List<CommunicationToken>(Communications);
                foreach (var token in tokens)
                {
                    int dos_protect = 30;
                    while (token.Receive(out var message))
                    {
                        message.ReceiverToken = token;
                        messages.Add(message);
                        
                        dos_protect--;
                        if (dos_protect < 0)
                        {
                            break;
                        }
                    }
                }
                
                await Pulse(messages);
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
            
        }
        
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(Dispose));
        }
    }
}
