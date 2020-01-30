
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    // TODO: need a way for subsystems to save communication contracts
    // they formed so they can send messages any time they want, like
    // saving to a database
    // TODO: need a way for subsystems to request to stay open for a few seconds
    // during the shutdown process so they can finish processing messages
    
    public abstract class BaseSubsystem : ISubsystem
    {
        public string Name { get; protected set; }
        public bool Disposed { get; private set; }
        public int MessageDenialOfServicePreventionCutoff { get; set; }
        internal List<CommunicationToken> Communications { get; set; }
        internal readonly object CommunicationsLock = new object();
        
        public BaseSubsystem()
        {
            Name = string.Empty;
            Disposed = false;
            Communications = new List<CommunicationToken>();
            MessageDenialOfServicePreventionCutoff = 30;
        }
        
        // TODO: cancellation token argument
        protected abstract void Pulse(Message message);
        
        public virtual Task Pulse()
        {
            return Task.Run(() =>
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
                        if (object.ReferenceEquals(message, Messages.Shutdown.HardShutdown))
                        {
                            // TODO: maybe a message or run some sort of method for the subsystem
                            // to clean up or something.
                            Log.Info($"{Name} received hard shutdown message");
                            Dispose();
                            return;
                        }
                        
                        Pulse(message);
                        
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
        
        public virtual Task AddCommunications(IEnumerable<CommunicationToken> tokens)
        {
            return Task.Run(() => 
            {
                lock (CommunicationsLock)
                {
                    foreach (var token in tokens)
                    {
                        Log.Debug($"{Name}: Adding communication token: {token}");
                        Communications.Add(token);
                    }
                }
            });
        }
        
        public virtual Task InformFault(Exception exception)
        {
            // TODO: create an internal CancellationTokenSource, have Pulse()
            // listen for it, then cancel it and call Pulse(Message) with a Fault
            // message.
            return Task.CompletedTask;
        }
        
        public virtual void Dispose()
        {
            Log.Debug("Disposing");
            
            if (Disposed)
            {
                Log.Warning($"{Name} is already disposed");
                return;
            }
            Disposed = true;
            
            List<CommunicationToken>? tokens = null;
            lock (CommunicationsLock)
            {
                tokens = new List<CommunicationToken>(Communications);
            }
            foreach (var token in tokens)
            {
                token.Dispose();
            }
            Communications.Clear();
            
            Log.Debug("Done disposing");
        }
        
        public virtual ValueTask DisposeAsync()
        {
            Log.Debug("Disposing asyncronously");
            return new ValueTask(Task.Run(Dispose));
        }
    }
}
