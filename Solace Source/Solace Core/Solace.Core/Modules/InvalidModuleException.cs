
namespace Solace.Core.Modules
{
    using System;
    
    public class InvalidModuleException : Exception
    {
        public InvalidModuleException() : base()
        {
            
        }
        
        public InvalidModuleException(string message) : base(message)
        {
            
        }
        
        public InvalidModuleException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}
