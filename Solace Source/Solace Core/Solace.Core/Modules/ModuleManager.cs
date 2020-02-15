
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Services;
    
    public class ModuleManager
    {
        public int QueueCheckDelay { get; set; }
        public event Func<string, IEnumerable<IService>, Task> OnModuleReady;
        public event Func<Exception, Task> OnError;
        
        private List<ModuleContainer> Containers { get; set; }
        private List<DependencyQueueToken> DependencyQueue { get; set; }
        
        public ModuleManager()
        {
            QueueCheckDelay = 1000;
            OnModuleReady   = delegate { return Task.CompletedTask; };
            OnError         = delegate { return Task.CompletedTask; };
            
            Containers      = new List<ModuleContainer>();
            DependencyQueue = new List<DependencyQueueToken>();
        }
        
        public void Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (DependencyQueue.Count() > 0)
                    {
                        var queue = new List<DependencyQueueToken>();
                        foreach (var dependency in queue)
                        {
                            await HandleDependencies(dependency);
                            
                            if (token.IsCancellationRequested)
                            {
                                break;
                            }
                        }
                    }
                    await Task.Delay(QueueCheckDelay);
                }
            });
        }
        
        public Task Load(string name, string path)
        {
            return Task.Run(async () =>
            {
                var container = new ModuleContainer(name);
                try
                {
                    var module = container.Load(name);
                    var services = module.GetServices();
                    Containers.Add(container);
                    
                    var dependency_token = new DependencyQueueToken(container, module.Dependencies);
                    DependencyQueue.Add(dependency_token);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Error loading module \"{name}\" from {path}");
                    await OnError.Invoke(e);
                }
            });
        }
        
        public Task Unload(string name)
        {
            return Task.Run(() =>
            {
                
            });
        }
        
        private Task HandleDependencies(DependencyQueueToken dependency_token)
        {
            return Task.Run(() =>
            {
                
            });
        }
        
        private Task StartServices(ModuleContainer container)
        {
            return Task.Run(async () =>
            {
                IService current = null!;
                try
                {
                    foreach (var service in container.Module!.GetServices())
                    {
                        current = service;
                        await service.Start(container.GetCancellationToken());
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Error starting service {current?.Name ?? "[None]"} in module {container.Module?.Info?.Name ?? "[None]"}");
                    await OnError.Invoke(e);
                }
            });
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
