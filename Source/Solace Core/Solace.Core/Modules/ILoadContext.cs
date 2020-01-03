
namespace Solace.Core.Modules
{
    using System;
    using System.Reflection;
    
    public interface ILoadContext : IDisposable
    {
        string? Name { get; }
        Assembly? Load(string path);
    }
}
