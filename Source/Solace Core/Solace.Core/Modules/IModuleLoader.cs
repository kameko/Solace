
namespace Solace.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IModuleLoader : IDisposable
    {
        bool TryLoadModule(string path, out BaseModule? module);
        BaseModule LoadModule(string path);
        string MoveTargetToTempFolder(string original, string destination);
        string MoveTargetToTempFolder(string original, string destination, bool random_name);
    }
}
