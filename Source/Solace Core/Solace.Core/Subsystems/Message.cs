
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class Message
    {
        public string Command { get; private set; }
        public string? Data { get; private set; }
        
        public Message(string command, string data)
        {
            Command = command;
            Data    = data;
        }
        
        public Message(string command) : this(command, null!)
        {
            
        }
    }
}
