
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public class SubsystemManager
    {
        private List<SubsystemContext> Subsystems { get; set; }
        private List<CommunicationContract> Contracts { get; set; }
        
        public SubsystemManager()
        {
            Subsystems = new List<SubsystemContext>();
            Contracts  = new List<CommunicationContract>();
        }
        
        public void Run(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Pulse();
                Thread.Sleep(15);
            }
        }
        
        public bool Add(ISubsystem subsystem)
        {
            if (Subsystems.Exists(x => x.Name.Equals(subsystem.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }
            
            var context = new SubsystemContext(subsystem);
            Subsystems.Add(context);
            Log.Info($"Adding subsystem {context.Name}");
            return true;
        }
        
        public bool Remove(string name)
        {
            var context = Subsystems.Find(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return Remove(name, context);
        }
        
        public bool Remove(ISubsystem subsystem)
        {
            var context = Subsystems.Find(x => x.Subsystem == subsystem);
            return Remove(subsystem.Name, context);
        }
        
        private bool Remove(string name, SubsystemContext? context)
        {
            if (context is null)
            {
                Log.Info($"Could not find subsystem {name} to remove");
                return false;
            }
            
            Log.Info($"Removing subsystem {context.Name}");
            var success = Subsystems.Remove(context);
            
            try
            {
                context.Subsystem.Dispose();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Subsystem {context.Name} encountered an error when disposing");
            }
            
            return success;
        }
        
        public bool RequestCommunicationContract(string requester_name, string subsystem_name, out CommunicationToken? token)
        {
            var context = Subsystems.Find(x => x.Name.Equals(subsystem_name, StringComparison.InvariantCultureIgnoreCase));
            if (context is null)
            {
                token = null;
                return false;
            }
            
            var cc = CommunicationContract.Create(requester_name, subsystem_name);
            
            context.TokenBuffer.Add(cc.Producer);
            
            token = cc.Subscriber;
            
            Contracts.Add(cc);
            return true;
        }
        
        public Task Pulse()
        {
            if (!Subsystems.Any(x => !x.Executing))
            {
                return Task.CompletedTask;
            }
            
            return Task
                .Run(InternalPulse)
                .ContinueWith(PulseContinuation);
        }
        
        private async Task InternalPulse()
        {
            if (Contracts.Count > 0)
            {
                Contracts.RemoveAll(x => x.Disposed);
            }
            
            List<SubsystemContext> subsystems = new List<SubsystemContext>(Subsystems);
            await InternalPulseSelect(subsystems);
        }
        
        private async Task InternalPulseSelect(List<SubsystemContext> subsystems)
        {
            List<Exception>? exceptions = null;
            
            foreach (var context in subsystems)
            {
                try
                {
                    if (!context.Executing)
                    {
                        context.Begin();
                        await context.ProcessTokens();
                        await context.PulseAsync();
                        context.End();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Subsystem {context.Name} encountered an unhandled error and will be removed");
                    
                    Remove(context.Name, context);
                    
                    if (exceptions is null)
                    {
                        exceptions = new List<Exception>();
                    }
                    exceptions.Add(e);
                }
            }
            
            if (!(exceptions is null))
            {
                var ae = new AggregateException(exceptions);
                ae = ae.Flatten();
                throw ae;
            }
        }
        
        private void PulseContinuation(Task task)
        {
            if (task.IsFaulted)
            {
                Log.Verbose(task.Exception!, string.Empty);
            }
        }
        
        private class SubsystemContext
        {
            public ISubsystem Subsystem { get; private set; }
            public bool Executing { get; private set; }
            public ConcurrentBag<CommunicationToken> TokenBuffer { get; private set; }
            
            public string Name => Subsystem.Name;
            
            public SubsystemContext(ISubsystem subsystem)
            {
                Subsystem   = subsystem;
                Executing   = false;
                TokenBuffer = new ConcurrentBag<CommunicationToken>();
            }
            
            public void Begin()
            {
                Executing = true;
            }
            
            public void End()
            {
                Executing = false;
            }
            
            public async Task PulseAsync()
            {
                await Subsystem.Pulse();
            }
            
            public Task ProcessTokens()
            {
                if (TokenBuffer.Count > 0)
                {
                    return Task
                        .Run(async () =>
                        {
                            var tokens = new List<CommunicationToken>(TokenBuffer);
                            await Subsystem.AddCommunications(tokens);
                        })
                        .ContinueWith(task =>
                        {
                            // clear the buffer regardless of if there was an error
                            // in AddCommunications, since whatever is in the buffer
                            // probably caused that error anyway.
                            TokenBuffer.Clear();
                        });
                }
                return Task.CompletedTask;
            }
        }
    }
}
