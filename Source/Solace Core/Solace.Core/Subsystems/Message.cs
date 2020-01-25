
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public static class Messages
    {
        public class ChannelClosed : Message
        {
            public static ChannelClosed Instance { get; private set; }
            public static string Message => "CHANNEL_CLOSED";
            
            static ChannelClosed()
            {
                Instance = new ChannelClosed();
            }
            
            public ChannelClosed() : base(Message)
            {
                
            }
        }
        
        public class Shutdown : Message<string>
        {
            public static Shutdown HardShutdown { get; private set; }
            public static string Message => "SHUTDOWN";
            
            static Shutdown()
            {
                HardShutdown = new Shutdown("HARD_SHUTDOWN");
            }
            
            public Shutdown() : base(Message)
            {
                Data = string.Empty;
            }
            
            public Shutdown(string reason) : base(Message, reason)
            {
                
            }
        }
    }
    
    public class Message<T> : Message
    {
        public T Data { get; protected set; }
        
        public Message(string command) : base(command)
        {
            Data = default!;
        }
        
        public Message(string command, T data) : this(command)
        {
            Data = data;
        }
        
        public Message(Message message) : this(string.Empty)
        {
            Copy(message);
        }
        
        public override void Copy(Message message)
        {
            base.Copy(message);
            if (message is Message<T> mt)
            {
                mt.Data = Data;
            }
        }
        
        public override string ToString()
        {
            if (Data?.Equals(default(T)!) ?? true)
            {
                return base.ToString();
            }
            else
            {
                return $"{base.ToString()}, {Data}";
            }
        }
    }
    
    public class Message
    {
        // These are set by CommunicationToken
        internal CommunicationToken SenderToken { get; set; }
        internal CommunicationToken ReceiverToken { get; set; }
        
        public string Sender => SenderToken.Name;
        public string Command { get; protected set; }
        
        public Message(string command)
        {
            SenderToken   = null!;
            ReceiverToken = null!;
            Command       = command;
        }
        
        public Message(Message message)
        {
            SenderToken   = null!;
            ReceiverToken = null!;
            Command       = null!;
            Copy(message);
        }
        
        public virtual void Copy(Message message)
        {
            SenderToken   = message.SenderToken;
            ReceiverToken = message.ReceiverToken;
            Command       = message.Command;
        }
        
        public void Respond(Message message)
        {
            ReceiverToken.Send(message);
        }
        
        public void CloseChannel()
        {
            ReceiverToken.Close();
        }
        
        public override string ToString()
        {
            if (SenderToken.Closed || ReceiverToken.Closed)
            {
                return $"{SenderToken.Name} -x-> {ReceiverToken.Name}: {Command}";
            }
            else
            {
                return $"{SenderToken.Name} ---> {ReceiverToken.Name}: {Command}";
            }
        }
    }
}
