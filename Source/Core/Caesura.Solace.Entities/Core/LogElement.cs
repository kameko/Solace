
namespace Caesura.Solace.Entities.Core
{
    using System;
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;
    using Contexts;
    
    // TODO: Add an Elements class/property like in Foundation.Logging.LogItem
    // TODO: way more information here
    // Items to add:
    // - Receiving service name
    // - Current resource usage
    
    public class LogElement : IId<ulong>
    {
        public ulong Id                   { get; set; }
        
        public DateTime TimeStamp         { get; set; }
        public LogLevel Level             { get; set; }
        public int EventId                { get; set; }
        public string Name                { get; set; } = string.Empty;
        public string Message             { get; set; } = string.Empty;
        public ExceptionElement Exception { get; set; }
        
        public LogElement()
        {
            Exception = new ExceptionElement();
        }
        
        public override string ToString()
        {
            return 
                $"[{Level}][{Name}({Id})]: {Message}{(string.IsNullOrEmpty(Message) ? string.Empty : " ")} " + 
                $"{(Exception is null ? string.Empty : Environment.NewLine)}{Exception}";
        }
        
        public class ExceptionElement
        {
            public ulong Id       { get; set; }
            public string Name    { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string Stack   { get; set; } = string.Empty;
            public bool HasValue  { get; set; } = false;
            
            public ExceptionElement()
            {
                
            }
            
            public ExceptionElement(Exception? ex)
            {
                if (!(ex is null))
                {
                    HasValue = true;
                    Name     = ex.GetType().FullName ?? string.Empty;
                    Message  = ex.Message;
                    Stack    = ex.StackTrace ?? string.Empty;
                }
            }
        }
    }
}
