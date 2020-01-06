
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public abstract class BaseModule
    {
        public abstract ModuleInfo Info { get; protected set; }
        protected EventRegistration Event { get; private set; }
        
        /// <summary>
        /// Called after all the module's dependencies are loaded and running.
        /// </summary>
        protected event Action OnStart;
        /// <summary>
        /// Called when the module is loaded into the system, including reloading.
        /// </summary>
        protected event Action OnLoad;
        /// <summary>
        /// Called when the module is being unloaded from the system.
        /// </summary>
        protected event Action OnUnload;
        
        public BaseModule()
        {
            Event     = null!;
            OnStart   = delegate { };
            OnLoad    = delegate { };
            OnUnload  = delegate { };
        }
        
        /// <summary>
        /// Setup the state of the module. Do not use the constructor.
        /// </summary>
        protected abstract void Setup(ModuleInit init);
        
        /// <summary>
        /// Called when the module is being reloaded. Returns any serialized state
        /// the module wishes to save to be restored later.
        /// </summary>
        /// <returns></returns>
        public virtual string PreReload()
        {
            // We use a string instead of an object because the system cannot hold on
            // to any dependencies from the module's DLL between reloads. The unfortunate
            // side-effect of this is that the module will be forced to serialized and
            // deserialize it's data instead of simply passing around a type from the module.
            return string.Empty;
        }
        
        /// <summary>
        /// Called after the module has been fully reloaded. Caller passes the serialized
        /// state that was previously handed to it by PreReload.
        /// </summary>
        /// <param name="state"></param>
        public virtual void PostReload(string state)
        {
            
        }
        
        internal void CallSetup(ModuleInit init)
        {
            Event = init.Events.Register(Info);
            Setup(init);
        }
        
        internal void RaiseOnStart()
        {
            OnStart?.Invoke();
        }
        
        internal void RaiseOnLoad()
        {
            OnLoad?.Invoke();
        }
        
        internal void RaiseOnUnload()
        {
            OnUnload?.Invoke();
        }
    }
}
