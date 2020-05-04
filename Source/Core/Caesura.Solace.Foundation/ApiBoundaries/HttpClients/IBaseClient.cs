
namespace Caesura.Solace.Foundation.ApiBoundaries.HttpClients
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    
    public interface IBaseClient
    {
        string Name { get; }
        
        Task<int> RequestPid();
        Task<int> RequestPid(CancellationToken token);
        Task<string> RequestShutdown(string reason);
        Task<string> RequestShutdown(string reason, CancellationToken token);
    }
}
