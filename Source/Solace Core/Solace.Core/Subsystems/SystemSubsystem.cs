
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    // Yes I am aware at how stupid this name is.
    
    public class SystemSubsystem : BaseSubsystem
    {
        internal SystemSubsystem() : base()
        {
            Name = "SYSTEM";
        }
        
        protected override Task Pulse(Message message)
        {
            // TODO:
            
            Log.Debug($"{Name} received message: {message}");
            
            return Task.CompletedTask;
        }
        
        public override Task Pulse()
        {
            return Task.Run(async () =>
            {
                List<CommunicationToken>? tokens = null;
                lock (CommunicationsLock)
                {
                    tokens = new List<CommunicationToken>(Communications);
                }
                foreach (var token in tokens)
                {
                    if (Disposed)
                    {
                        return;
                    }
                    if (token.Closed)
                    {
                        lock (CommunicationsLock)
                        {
                            Communications.Remove(token);
                        }
                    }
                    
                    int dos_protect = MessageDenialOfServicePreventionCutoff;
                    while (token.Receive(out var message))
                    {
                        if (Disposed)
                        {
                            return;
                        }
                        
                        await Pulse(message);
                        
                        dos_protect--;
                        if (dos_protect < 0)
                        {
                            Log.Debug(
                                $"{Name}: Denial of service cutoff ({MessageDenialOfServicePreventionCutoff}) " +
                                $"from {token.Name} reached" 
                            );
                            break;
                        }
                    }
                }
            });
        }
    }
}
