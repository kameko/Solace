
namespace Caesura.Solace.Entities.Core
{
    using System;
    using Microsoft.Extensions.Logging;
    
    // TODO: Add an Elements class/property like in Foundation.Logging.LogItem
    
    public class LogElement
    {
        public ulong Id             { get; set; }
        
        public DateTime TimeStamp   { get; set; }
        public LogLevel Level       { get; set; }
        public int EventId          { get; set; }
        public string Name          { get; set; } = string.Empty;
        public string Message       { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
        
        
        
        public override string ToString()
        {
            return 
                $"[{Level}][{Name}({Id})]: {Message}{(string.IsNullOrEmpty(Message) ? string.Empty : " ")} " + 
                $"{(Exception is null ? string.Empty : Environment.NewLine)}{Exception}";
        }
    }
}
