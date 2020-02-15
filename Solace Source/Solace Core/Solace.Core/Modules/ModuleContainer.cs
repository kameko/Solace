
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
        internal ModuleLoader? Loader { get; set; }
        internal BaseModule? Module { get; set; }
        private CancellationTokenSource? Token { get; set; }
        
        public ModuleContainer(string name)
        {
            Name = name;
        }
        
        internal CancellationToken GetCancellationToken()
        {
            Token = Token ?? new CancellationTokenSource();
            return Token.Token;
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
        
        public bool TryLoad(string path)
        {
            return TryLoad(path, out var _);
        }
        
        public void Unload()
        {
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
