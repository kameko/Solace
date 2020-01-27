
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Channels;
    
    public class CommunicationToken : IDisposable, IAsyncDisposable
    {
        internal CommunicationContract Contract { get; set; }
        internal CommunicationToken Other { get; set; }
        public string Name { get; private set; }
        public bool Closed { get; private set; }
        internal Channel<Message> Input { get; set; }
        internal Channel<Message> Output { get; set; }
        
        internal CommunicationToken(string name, Channel<Message> input, Channel<Message> output)
        {
            Contract = null!;
            Other    = null!;
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
        
        public async ValueTask<Message> ReceiveAsync(CancellationToken cancel_token)
        {
            return await Input.Reader.ReadAsync(cancel_token);
        }
        
        public IAsyncEnumerable<Message> ReceiveAllAsync()
        {
            return Input.Reader.ReadAllAsync();
        }
        
        public IAsyncEnumerable<Message> ReceiveAllAsync(CancellationToken cancel_token)
        {
            return Input.Reader.ReadAllAsync(cancel_token);
        }
        
        public bool Send(Message message)
        {
            if (Closed)
            {
                return false;
            }
            
            if (!(message.SenderToken is null) || !(message.ReceiverToken is null))
            {
                throw new InvalidOperationException(
                    "Message sender and receiver must be null. If they are not, you may " +
                    "be attempting to send the same message instance to two different " +
                    "subsystems which can lead to data corruption"
                );
            }
            
            message.SenderToken   = this;
            message.ReceiverToken = Other;
            return Output.Writer.TryWrite(message);
        }
        
        /// <summary>
        /// Closes the output channel, preventing sending any more messages.
        /// Note that this token can still receive messages sent to it until
        /// the other token is closed. Always call this when finished communicating
        /// with another token to allow the tokens to be disposed.
        /// </summary>
        public void Close()
        {
            Send(new Messages.ChannelClosed());
            Output.Writer.TryComplete();
            Closed = true;
        }
        
        public void Dispose()
        {
            Close();
        }
        
        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(Dispose));
        }
        
        public override string ToString()
        {
            string relationship = string.Empty;
            
            if (Closed && Other.Closed)
            {
                relationship = "<-x->";
            }
            else if (Closed)
            {
                relationship = "<--";
            }
            else if (Other.Closed)
            {
                relationship = "-->";
            }
            else
            {
                relationship = "<--->";
            }
            
            return $"{Name} {relationship} {Other.Name}";
        }
    }
}
