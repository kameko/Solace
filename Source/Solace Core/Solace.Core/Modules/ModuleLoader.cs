
namespace Solace.Core.Modules
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    
    public class ModuleLoader : IModuleLoader
    {
        public string Name => Loader.Name ?? string.Empty;
        
        private ILoadContext Loader { get; set; }
        private IIOPlatform IO { get; set; }
        
        public ModuleLoader(string name)
        {
            Loader = new LoadContext(name);
            IO     = new IOPlatform();
        }
        
        public ModuleLoader() : this(Guid.NewGuid().ToString().ToUpper())
        {
            
        }
        
        public ModuleLoader(ILoadContext loader, IIOPlatform io)
        {
            Loader = loader;
            IO     = io;
        }
        
        public bool TryLoadModule(string path, out BaseModule? module)
        {
            var result = InternalLoadModule(path);
            if (result.Success)
            {
                module = result.Module;
                return true;
            }
            else
            {
                module = null;
                return false;
            }
        }
        
        public BaseModule LoadModule(string path)
        {
            var result = InternalLoadModule(path);
            if (result.Success)
            {
                return result.Module!;
            }
            else
            {
                throw new InvalidModuleException(result.Error, result.Exception!);
            }
        }
        
        private (bool Success, BaseModule? Module, string Error, Exception? Exception) InternalLoadModule(string path)
        {
            if (!IO.FileExists(path))
            {
                return (false, null, $"No file found at \"{path}\"", null);
            }
            
            Assembly? assembly = null;
            try
            {
                assembly = Loader.Load(path);
            }
            catch (Exception e)
            {
                // catches:
                // ArgumentNullException
                // FileNotFoundException
                // BadImageFormatException
                // FileLoadException
                return (false, null, $"{nameof(ILoadContext)}: {e.GetType()}: {e.Message}", e);
            }
            
            if (assembly is null)
            {
                return (false, null, $"Could not load module at \"{path}\"", null);
            }
            
            var modules = assembly.GetExportedTypes().Where(x => x.IsSubclassOf(typeof(BaseModule)));
            var count = modules.Count();
            if (count > 1)
            {
                return (false, null, $"More than one type inheriting from {nameof(BaseModule)} detected in assembly", null);
            }
            if (count == 0)
            {
                return (false, null, $"No types inheriting from {nameof(BaseModule)} found in assembly", null);
            }
            
            object? tempobj = null;
            try
            {
                tempobj = Activator.CreateInstance(modules.First());
            }
            catch (Exception e)
            {
                // catches:
                // ArgumentNullException
                // ArgumentException
                // NotSupportedException
                // TargetInvocationException
                // MethodAccessException
                // MemberAccessException
                // InvalidComObjectException
                // MissingMethodException
                // COMException
                // TypeLoadException
                return (false, null, $"{nameof(Activator.CreateInstance)}: {e.GetType()}: {e.Message}", e);
            }
            
            if (tempobj is null)
            {
                return (false, null, $"Could not create an instance of module", null);
            }
            
            try
            {
                var module = (BaseModule)tempobj;
                return (true, module, string.Empty, null);
            }
            catch (InvalidCastException e)
            {
                return (false, null, $"Could not cast module of type {tempobj.GetType()} to {nameof(BaseModule)}", e);
            }
        }
        
        public string MoveTargetToTempFolder(string original, string destination) => 
            MoveTargetToTempFolder(original, destination, true);
        
        public string MoveTargetToTempFolder(string original, string destination, bool random_name)
        {
            var mod_folder = Path.Combine(
                new DirectoryInfo(destination).FullName, 
                $"{Path.GetFileNameWithoutExtension(original)}"
            );
            
            if (random_name)
            {
                var date = DateTime.UtcNow;
                var timestamp = date.ToString()
                    .Replace('/', '-')
                    .Replace(':', '-')
                    .Replace(' ', '-');
                mod_folder = $"{mod_folder}-{timestamp}";
                // mod_folder = $"{mod_folder}-{date.Millisecond}";
            }
            
            if (IO.DirectoryExists(mod_folder))
            {
                // clean module temp folder on startup to prevent outdated copies.
                IO.RecursivelyDeleteDirectory(mod_folder);
            }
            
            // create the module folder. IO.RecursivelyDeleteDirectory always deletes the folder passed to it.
            IO.CreateDirectory(mod_folder);
            
            var original_folder = new FileInfo(original).Directory.FullName;
            IO.RecursivelyCopyDirectory(original_folder, mod_folder);
            
            return Path.Combine(mod_folder, new FileInfo(original).Name);
        }
        
        public void Dispose()
        {
            Loader.Dispose();
        }
    }
}
