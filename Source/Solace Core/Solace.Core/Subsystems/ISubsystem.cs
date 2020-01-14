
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading.Channels;
    
    public interface ISubsystem : IDisposable, IAsyncDisposable
    {
        SubsystemExecutorOptions GetExecutorOptions();
        Func<Task>? GetExecutor();
        ChannelReader<object> Subscribe(string name);
        ChannelReader<object> Subscribe(string name, int capacity);
        bool Unsubscribe(string name);
        void PlugInput(ChannelReader<object> reader);
    }
}
