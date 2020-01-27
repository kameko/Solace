
namespace Solace.Core.Modules
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Loader;
    
    internal class LoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver? _resolver;
        
        public LoadContext(string name, string modulePath, bool isCollectible) : base(name: name, isCollectible: isCollectible)
        {
            _resolver = new AssemblyDependencyResolver(modulePath);
            CommonConstructor();
        }
        
        public LoadContext(string name) : base(name: name, isCollectible: true)
        {
            CommonConstructor();
        }
        
        private void CommonConstructor()
        {
            Unloading += alc => Log.Debug($"{nameof(AssemblyLoadContext)} \"{alc.Name}\" has unloaded.");
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (this._resolver is null)
            {
                return null;
            }
            string? assemblyPath = this._resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return this.LoadFromAssemblyPath(assemblyPath);
            }
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            if (this._resolver is null)
            {
                return IntPtr.Zero;
            }
            string? libraryPath = this._resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return this.LoadUnmanagedDllFromPath(libraryPath);
            }
            return IntPtr.Zero;
        }
        
        public Assembly? Load(string path)
        {
            _resolver = new AssemblyDependencyResolver(path);
            var asmname = new AssemblyName(Path.GetFileNameWithoutExtension(path));
            return this.Load(asmname);
        }
        
        public void Dispose()
        {
            if (IsCollectible)
            {
                this.Unload();
            }
        }
    }
}
