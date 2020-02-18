
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
        public event Func<string, IEnumerable<IService>, Task> OnServicesFound;
        public event Func<string, IEnumerable<string>, Task> OnRequestStopServices;
        public event Func<Exception, Task> OnError;
        
        // TODO: consider making these thread safe, although I doubt the system
        // will ever race to load two moduels at once.
        private ConfigurationManager Config { get; set; }
        private List<ModuleContainer> Containers { get; set; }
        private List<DependencyQueueToken> DependencyQueue { get; set; }
        private CancellationTokenSource? Token { get; set; }
        
        public ModuleManager(ConfigurationManager config)
        {
            QueueCheckDelay       = 500;
            OnModuleReady         = delegate { return Task.CompletedTask; };
            OnError               = delegate { return Task.CompletedTask; };
            OnServicesFound       = delegate { return Task.CompletedTask; };
            OnRequestStopServices = delegate { return Task.CompletedTask; };
            
            Config                = config;
            Containers            = new List<ModuleContainer>();
            DependencyQueue       = new List<DependencyQueueToken>();
        }
        
        public void Start(Dictionary<string, string> modules)
        {
            Token = Token ?? new CancellationTokenSource();
            Start(modules, Token.Token);
        }
        
        public void Stop()
        {
            Token?.Cancel();
        }
        
        private void Start(Dictionary<string, string> modules, CancellationToken token)
        {
            Task.Run(async () =>
            {
                foreach (var kvp in modules)
                {
                    await Load(kvp.Key, kvp.Value);
                    
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
                
                while (!token.IsCancellationRequested)
                {
                    if (DependencyQueue.Count() > 0)
                    {
                        var queue = new List<DependencyQueueToken>(DependencyQueue);
                        foreach (var dependency in queue)
                        {
                            await CheckAndWaitForDependencies(dependency);
                            
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
                if (name == "[NONE]" || path == "[NONE]")
                {
                    return;
                }
                
                Log.Info($"Loading module \"{name}\" from {path}");
                
                if (Containers.Exists(x => x.Name == name))
                {
                    Log.Warning($"Module {name} already loaded");
                    return;
                }
                
                var container = new ModuleContainer(name);
                try
                {
                    var module   = container.Load(path);
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
                        await RaiseOnServicesFound(container);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Error loading module \"{name}\" from {path}");
                    await OnError.Invoke(e);
                }
            });
        }
        
        public async Task Reload(string name)
        {
            var container = Containers.Find(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (!(container is null))
            {
                var path = container.Path;
                await Unload(container);
                await Load(name, path);
            }
        }
        
        public async Task Unload(string name)
        {
            var container = Containers.Find(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (!(container is null))
            {
                await Unload(container);
            }
        }
        
        private Task Unload(ModuleContainer container)
        {
            return Task.Run(async () =>
            {
                var dependers = Containers.FindAll(x => x.Module!.Dependencies.Contains(container.Name));
                foreach (var depender in dependers)
                {
                    await Unload(depender);
                }
                
                Containers.Remove(container);
                DependencyQueue.RemoveAll(x => object.ReferenceEquals(x.Container, container));
                
                GC.Collect();
                await Task.Delay(300);
                
                await OnRequestStopServices.Invoke(container.Module!.Info.Name, container.Module.GetServices().Select(x => x.Name));
            });
        }
        
        private Task CheckAndWaitForDependencies(DependencyQueueToken dependency_token)
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
                    await RaiseOnServicesFound(dependency_token.Container);
                }
            });
        }
        
        private Task RaiseOnServicesFound(ModuleContainer container)
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
                    
                    await OnServicesFound.Invoke(container.Module.Info.Name, services);
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
