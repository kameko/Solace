
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class Message
    {
        internal CommunicationToken SenderToken { get; set; }
        internal CommunicationToken? ReceiverToken { get; set; }
        
        public string Sender => SenderToken.Name;
        public string Command { get; private set; }
        public string? Data { get; private set; }
        
        public Message(string command, string data)
        {
            SenderToken = null!;
            Command     = command;
            Data        = data;
        }
        
        public Message(string command) : this(command, null!)
        {
            
        }
        
        public Message(Message message)
        {
            SenderToken = null!;
            Command     = null!;
            Copy(message);
        }
        
        public Message Copy()
        {
            var message           = new Message(Command, Data!);
            message.SenderToken   = SenderToken;
            message.ReceiverToken = ReceiverToken;
            return message;
        }
        
        public void Copy(Message message)
        {
            SenderToken   = message.SenderToken;
            ReceiverToken = message.ReceiverToken;
            Command       = message.Command;
            Data          = message.Data;
        }
        
        public void Respond(Message message)
        {
            SenderToken.Send(message);
        }
        
        public void Respond(string command)
        {
            var message = new Message(command);
            Respond(message);
        }
        
        public void Respond(string command, string data)
        {
            var message = new Message(command, data);
            Respond(message);
        }
        
        public void CloseChannel()
        {
            if (!(ReceiverToken is null))
            {
                ReceiverToken.Close();
            }
        }
    }
}
