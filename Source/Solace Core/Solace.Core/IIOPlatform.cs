
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IIOPlatform
    {
        void CreateDirectory(string name);
        IEnumerable<string> GetDirectories(string path);
        IEnumerable<string> EnumerateDirectories(string path);
        IEnumerable<string> GetFiles(string path);
        IEnumerable<string> EnumerateFiles(string path);
        void Copy(string original, string target);
        void DeleteFile(string file);
        void DeleteDirectory(string path);
        void RecursivelyCopyDirectory(string original, string target);
        void RecursivelyDeleteDirectory(string path);
        IEnumerable<string> RecursivelyGetFiles(string path);
        bool DirectoryExists(string path);
        bool FileExists(string path);
    }
}
