
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public class SubsystemManager
    {
        private List<SubsystemContext> Subsystems { get; set; }
        
        public SubsystemManager()
        {
            Subsystems = new List<SubsystemContext>();
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
            return Subsystems.Remove(context);
        }
        
        public Task Pulse()
        {
            var select = Subsystems.Any(x => !x.Executing);
            if (select)
            {
                return Task
                    .Run(InternalPulse)
                    .ContinueWith(HandlePulseException);
            }
            return Task.CompletedTask;
        }
        
        private async Task InternalPulse()
        {
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
            
            // return Task.CompletedTask;
        }
        
        private void HandlePulseException(Task task)
        {
            if (task.IsFaulted)
            {
                // Log.Error(task.Exception!, string.Empty);
            }
        }
        
        private class SubsystemContext
        {
            public ISubsystem Subsystem { get; private set; }
            public bool Executing { get; private set; }
            
            public string Name => Subsystem.Name;
            
            public SubsystemContext(ISubsystem subsystem)
            {
                Subsystem = subsystem;
                Executing = false;
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
        }
    }
}
