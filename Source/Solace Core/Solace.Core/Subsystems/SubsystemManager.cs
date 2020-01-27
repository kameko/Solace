
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
        internal SystemSubsystem SystemSubsystem { get; set; }
        private List<SubsystemContext> Subsystems { get; set; }
        private List<CommunicationContract> Contracts { get; set; }
        private readonly object SubsystemsLock = new object();
        private readonly object ContractsLock = new object();
        public event Func<ISubsystem, Task> OnSubsystemFailure;
        
        public SubsystemManager()
        {
            Subsystems         = new List<SubsystemContext>();
            Contracts          = new List<CommunicationContract>();
            SystemSubsystem    = new SystemSubsystem();
            OnSubsystemFailure = delegate { return Task.CompletedTask; };
            
            var sssc = new SubsystemContext(SystemSubsystem);
            Subsystems.Add(sssc);
        }
        
        public void Run(CancellationToken token)
        {
            Run(token, 15);
        }
        
        public void Run(CancellationToken token, int sleep)
        {
            while (!token.IsCancellationRequested)
            {
                Pulse();
                Thread.Sleep(sleep);
            }
        }
        
        public bool Add(ISubsystem subsystem)
        {
            lock (SubsystemsLock)
            {
                if (Subsystems.Exists(x => x.Name.Equals(subsystem.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return false;
                }
                
                var context = new SubsystemContext(subsystem);
                Subsystems.Add(context);
                Log.Info($"Adding subsystem {context.Name}");
                
                var contract_success = FormCommunicationContract(SystemSubsystem.Name, subsystem.Name, out var contract);
                if (contract_success)
                {
                    contract!.AsSubscriber(new Messages.Start());
                }
                else
                {
                    Log.Error(
                        $"Forming communication contract with the primary " + 
                        $"System subsystem and new subsystem {subsystem.Name} failed. " +
                        $"Removing failed subsystem from system"
                    );
                    Subsystems.Remove(context);
                    return false;
                }
                
                return true;
            }
        }
        
        public bool Remove(string name)
        {
            lock (SubsystemsLock)
            {
                var context = Subsystems.Find(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                return Remove(name, context);
            }
        }
        
        public bool Remove(ISubsystem subsystem)
        {
            lock (SubsystemsLock)
            {
                var context = Subsystems.Find(x => x.Subsystem == subsystem);
                return Remove(subsystem.Name, context);
            }
        }
        
        private bool Remove(string name, SubsystemContext? context)
        {
            if (context is null)
            {
                Log.Info($"Could not find subsystem {name} to remove");
                return false;
            }
            
            Log.Info($"Removing subsystem {context.Name}");
            
            var success = false;
            lock (SubsystemsLock)
            {
                success = Subsystems.Remove(context);
            }
            
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
            // sanity check
            if (requester_name.Equals(subsystem_name, StringComparison.InvariantCultureIgnoreCase))
            {
                token = null;
                return false;
            }
            
            SubsystemContext? context = null;
            
            lock (SubsystemsLock)
            {
                context = Subsystems.Find(x => x.Name.Equals(subsystem_name, StringComparison.InvariantCultureIgnoreCase));
                if (context is null)
                {
                    token = null;
                    return false;
                }
            }
            
            var cc = CommunicationContract.Create(requester_name, subsystem_name);
            
            context.TokenBuffer.Add(cc.Producer);
            
            token = cc.Subscriber;
            
            lock (ContractsLock)
            {
                Contracts.Add(cc);
            }
            
            return true;
        }
        
        public bool FormCommunicationContract(string subscriber_name, string producer_name, out CommunicationContract? contract)
        {
            if (subscriber_name.Equals(producer_name, StringComparison.InvariantCultureIgnoreCase))
            {
                contract = null;
                return false;
            }
            
            SubsystemContext? subscriber = null;
            SubsystemContext? producer   = null;
            
            lock (SubsystemsLock)
            {
                subscriber = Subsystems.Find(x => x.Name.Equals(subscriber_name, StringComparison.InvariantCultureIgnoreCase));
                producer   = Subsystems.Find(x => x.Name.Equals(producer_name  , StringComparison.InvariantCultureIgnoreCase));
                if (subscriber is null || producer is null)
                {
                    contract = null;
                    return false;
                }
            }
            
            var cc = CommunicationContract.Create(subscriber_name, producer_name);
            
            subscriber.TokenBuffer.Add(cc.Subscriber);
            producer.TokenBuffer.Add(cc.Producer);
            
            lock (ContractsLock)
            {
                Contracts.Add(cc);
            }
            
            contract = cc;
            return true;
        }
        
        public Task Pulse()
        {
            lock (SubsystemsLock)
            {
                if (!Subsystems.Any(x => !x.Executing))
                {
                    return Task.CompletedTask;
                }
            }
            
            return Task
                .Run(InternalPulse)
                .ContinueWith(PulseContinuation);
        }
        
        private async Task InternalPulse()
        {
            lock (ContractsLock)
            {
                if (Contracts.Count > 0)
                {
                    Contracts.RemoveAll(x => x.Closed);
                }
            }
            
            List<SubsystemContext>? subsystems = null;
            
            lock (SubsystemsLock)
            {
                subsystems = new List<SubsystemContext>(Subsystems);
            }
            
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
                    
                    lock (SubsystemsLock)
                    {
                        Remove(context.Name, context);
                    }
                    
                    if (exceptions is null)
                    {
                        exceptions = new List<Exception>();
                    }
                    exceptions.Add(e);
                    
                    try
                    {
                        await OnSubsystemFailure(context.Subsystem);
                    }
                    catch (Exception ei)
                    {
                        Log.Error(ei, $"Calling OnSubsystemFailure raised an error when removing subsystem {context.Name}");
                    }
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
                Log.Verbose(task.Exception!, "Pulse ended with unhandled exception");
            }
        }
        
        private class SubsystemContext
        {
            // TODO: more metadata, like when the subsystem was loaded
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
