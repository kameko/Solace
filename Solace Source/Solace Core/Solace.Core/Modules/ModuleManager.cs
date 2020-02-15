
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
        private CancellationTokenSource? Token { get; set; }
        
        public ModuleManager()
        {
            QueueCheckDelay = 1000;
            OnModuleReady   = delegate { return Task.CompletedTask; };
            OnError         = delegate { return Task.CompletedTask; };
            
            Containers      = new List<ModuleContainer>();
            DependencyQueue = new List<DependencyQueueToken>();
        }
        
        public void Start()
        {
            Token = Token ?? new CancellationTokenSource();
            Start(Token.Token);
        }
        
        public void Stop()
        {
            Token?.Cancel();
        }
        
        private void Start(CancellationToken token)
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
                Log.Info($"Loading module \"{name}\" from {path}");
                var container = new ModuleContainer(name);
                try
                {
                    var module = container.Load(name);
                    var services = module.GetServices();
                    Containers.Add(container);
                    
                    if (module.Dependencies.Count > 0)
                    {
                        Log.Info($"Module \"{name}\" has dependencies. Queuing for dependency verification");
                        var dependency_token = new DependencyQueueToken(container, module.Dependencies);
                        DependencyQueue.Add(dependency_token);
                    }
                    else
                    {
                        await StartServices(container);
                    }
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
                var container = Containers.Find(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (!(container is null))
                {
                    container.Stop();
                    Containers.Remove(container);
                    DependencyQueue.RemoveAll(x => object.ReferenceEquals(x.Container, container));
                }
            });
        }
        
        private Task HandleDependencies(DependencyQueueToken dependency_token)
        {
            return Task.Run(async () =>
            {
                var deps        = dependency_token.Container.Module!.Dependencies;
                var deps_count  = deps.Count;
                var deps_loaded = 0;
                
                foreach (var dep in deps)
                {
                    if (Containers.Exists(x => x.Module!.Info.Name.Equals(dep, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        deps_loaded++;
                    }
                }
                
                if (deps_loaded >= deps_count)
                {
                    Log.Info($"All dependencies loaded for module {dependency_token.Container.Module.Info.Name}");
                    DependencyQueue.Remove(dependency_token);
                    await StartServices(dependency_token.Container);
                }
            });
        }
        
        private Task StartServices(ModuleContainer container)
        {
            return Task.Run(async () =>
            {
                IService current = null!;
                try
                {
                    Log.Info($"Starting services from module \"{container.Module!.Info.Name}\"");
                    var services = container.Module!.GetServices();
                    if (services.Count() == 0)
                    {
                        Log.Warning($"Module \"{container.Module!.Info.Name}\" has no services. You should consider adding one");
                        return;
                    }
                    foreach (var service in services)
                    {
                        Log.Info($"Starting service \"{service.Name}\" from module \"{container.Module!.Info.Name}\"");
                        current = service;
                        await service.Start(container.GetCancellationToken());
                    }
                    Log.Info($"Finished starting services from module \"{container.Module!.Info.Name}\"");
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Error starting service {current?.Name ?? "[None]"} in module \"{container.Module?.Info?.Name ?? "[None]"}\"");
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
