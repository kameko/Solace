
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Modules;
    
    public delegate Task EventCallback(ModuleMessage message);
    
    public class EventRegistration
    {
        private Events Events { get; set; }
        private ModuleInfo Owner { get; set; }
        
        public EventRegistration(Events events, ModuleInfo owner)
        {
            Events = events;
            Owner  = owner;
        }
        
        public void Subscribe(string name, EventCallback callback)
        {
            var token = new EventToken(name, Owner, callback);
            Events.AddToken(token);
        }
        
        public void Unsubscribe(string name, EventCallback callback)
        {
            Events.RemoveToken(name, Owner, callback);
        }
        
        public void UnsubscribeAll(string name)
        {
            Events.RemoveAllTokens(name, Owner);
        }
        
        public void Raise(string name)
        {
            Events.Raise(name, Owner);
        }
        
        public void Raise(string name, object args)
        {
            Events.Raise(name, Owner, args);
        }
        
        public void Block(string name)
        {
            Events.Block(name, Owner);
        }
        
        public void Release(string name)
        {
            Events.Release(name, Owner);
        }
    }
}
