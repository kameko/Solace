
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Services;
    
    public class ModuleManager
    {
        private List<ModuleContainer> Containers { get; set; }
        
        public ModuleManager()
        {
            Containers = new List<ModuleContainer>();
        }
        
        public IEnumerable<IService> Load(string name, string path)
        {
            var container = new ModuleContainer(name);
            try
            {
                var module = container.Load(name);
                var services = module.GetServices();
                Containers.Add(container);
                
                // TODO: handle dependencies.
                
                container.Run();
                return services;
            }
            catch
            {
                // TODO: log failure
                // TODO: some sort of OnFailure callback
                return new List<IService>();
            }
        }
        
        public void Unload(string name)
        {
            
        }
        
        private class DependencyQueueToken
        {
            public ModuleContainer Container { get; set; }
            public List<string> DependenciesLeft { get; set; }
            
            public DependencyQueueToken(ModuleContainer container, List<string> dependencies)
            {
                Container        = container;
                DependenciesLeft = dependencies;
            }
        }
    }
}
