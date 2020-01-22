
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Channels;
    
    public class CommunicationToken : IDisposable, IAsyncDisposable
    {
        internal CommunicationContract Contract { get; set; }
        public string Name { get; private set; }
        public bool Disposed { get; private set; }
        internal Channel<Message> Input { get; set; }
        internal Channel<Message> Output { get; set; }
        
        public CommunicationToken(string name, Channel<Message> input, Channel<Message> output)
        {
            Contract = null!;
            Name     = name;
            Input    = input;
            Output   = output;
        }
        
        public bool Receive(out Message message)
        {
            return Input.Reader.TryRead(out message);
        }
        
        public async ValueTask<Message> ReceiveAsync()
        {
            return await Input.Reader.ReadAsync();
        }
        
        public IAsyncEnumerable<Message> ReceiveAllAsync()
        {
            return Input.Reader.ReadAllAsync();
        }
        
        public bool Send(Message message)
        {
            message.SenderToken = this;
            return Output.Writer.TryWrite(message);
        }
        
        public void Close()
        {
            Dispose();
        }
        
        public void Dispose()
        {
            Output.Writer.TryComplete();
            Disposed = true;
        }
        
        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(Dispose));
        }
    }
}
