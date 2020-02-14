
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public class ModuleContainer : IDisposable
    {
        public string Name { get; set; }
        private CancellationTokenSource? Token { get; set; }
        private ModuleLoader? Loader { get; set; }
        private BaseModule? Module { get; set; }
        
        public ModuleContainer(string name)
        {
            Name = name;
        }
        
        public BaseModule Load(string path)
        {
            Loader = new ModuleLoader(Name);
            Module = Loader.LoadModule(path);
            return Module;
        }
        
        public bool TryLoad(string path, out BaseModule? module)
        {
            Loader = new ModuleLoader(Name);
            var success = Loader.TryLoadModule(path, out module);
            Module = module;
            return success;
        }
        
        public void Run()
        {
            Token = new CancellationTokenSource();
            var services = Module?.GetServices();
            foreach (var service in services!)
            {
                service.Start(Token.Token);
            }
        }
        
        public void Stop()
        {
            Token?.Cancel();
        }
        
        public bool TryLoad(string path)
        {
            return TryLoad(path, out var _);
        }
        
        public void Unload()
        {
            Stop();
            Module = null;
            Loader?.Dispose();
            Loader = null;
            GC.Collect();
        }
        
        public void Dispose()
        {
            Unload();
        }
    }
}
