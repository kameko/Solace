
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface ISubsystem : IDisposable, IAsyncDisposable
    {
        string Name { get; }
        Task Pulse();
    }
}
