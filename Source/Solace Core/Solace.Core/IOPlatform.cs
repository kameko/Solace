
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;
    
    public class IOPlatform : IIOPlatform
    {
        public void CreateDirectory(string name)
        {
            Directory.CreateDirectory(name);
        }
        
        public IEnumerable<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
        
        public IEnumerable<string> EnumerateDirectories(string path)
        {
            return Directory.EnumerateDirectories(path);
        }
        
        public IEnumerable<string> GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }
        
        public IEnumerable<string> EnumerateFiles(string path)
        {
            return Directory.EnumerateFiles(path);
        }
        
        public void Copy(string original, string target)
        {
            File.Copy(original, target);
            File.SetAttributes(target, FileAttributes.Normal);
        }
        
        public void DeleteFile(string file)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }
        
        public void DeleteDirectory(string path)
        {
            Directory.Delete(path);
        }
        
        public void RecursivelyCopyDirectory(string original, string target)
        {
            RecursivelyCopyDirectory(original, target, 0);
        }
        
        private void RecursivelyCopyDirectory(string original, string target, int stackSize)
        {
            const int maxstack = 100;  // Arbitrary number, no sane module will have 100 nested folders.
            if (stackSize > maxstack)
            {
                throw new InvalidOperationException($"Directory depth too large, cannot have folders nested further than {maxstack}");
            }
            
            DirectoryInfo dir = new DirectoryInfo(original);
            
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {original}");
            }
            
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
            
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                string temppath = Path.Combine(target, file.Name);
                file.CopyTo(temppath, false);
                File.SetAttributes(temppath, FileAttributes.Normal);
            }
            
            var dirs = dir.GetDirectories();
            foreach (var subdir in dirs)
            {
                string temppath = Path.Combine(target, subdir.Name);
                RecursivelyCopyDirectory(subdir.FullName, temppath, stackSize + 1);
            }
        }
        
        public void RecursivelyDeleteDirectory(string path)
        {
            var files = RecursivelyGetFiles(path);
            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                Directory.Delete(dir);
            }
            Directory.Delete(path);
        }
        
        public IEnumerable<string> RecursivelyGetFiles(string path)
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        }
        
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
