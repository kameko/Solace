
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Modules;
    
    public class EventToken
    {
        public string EventName { get; set; }
        public ModuleInfo Module { get; set; }
        public EventCallback Callback { get; set; }
        
        public bool Faulted => !(Exceptions is null);
        
        private AggregateException? Exceptions;
        private readonly object ExceptionsLock = new object();
        
        public EventToken(string name, ModuleInfo module, EventCallback callback)
        {
            EventName = name;
            Module    = module;
            Callback  = callback;
        }
        
        public void Execute(CancellationToken ct, ModuleMessage message)
        {
            Task.Run(async () =>
                {
                    await Callback(message);
                },
                ct
            ).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        lock (ExceptionsLock)
                        {
                            var ex = new AggregateException(
                                new AggregateException[] 
                                {
                                    Exceptions!,
                                    task.Exception!
                                }
                            );
                            Exceptions = ex.Flatten();
                        }
                    }
                }
            );
        }
        
        public AggregateException? GetException()
        {
            lock (ExceptionsLock)
            {
                if (Faulted)
                {
                    var ex = Exceptions!;
                    Exceptions = null;
                    return ex;
                }
                return null;
            }
        }
    }
}
