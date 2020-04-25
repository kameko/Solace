
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    
    public class LogItem
    {
        public SolaceConsoleLoggerConfiguration Configuration { get; set; }
        public DateTime TimeStamp { get; set; }
        public LogLevel Level { get; set; }
        public EventId Id { get; set; }
        public string Name { get; set; }
        public object? State { get; set; }
        public Exception? Exception { get; set; }
        
        public LogItem(SolaceConsoleLoggerConfiguration config, LogLevel logLevel, EventId eventId, string name, object? state, Exception? exception)
        {
            TimeStamp     = DateTime.UtcNow;
            Configuration = config;
            Level         = logLevel;
            Id            = eventId;
            Name          = name;
            State         = state;
            Exception     = exception;
        }
        
        public override string ToString()
        {
            return 
                $"[{Level}]({Id}): {State}{(State is null ? string.Empty : " ")} " + 
                $"{(Exception is null ? string.Empty : Environment.NewLine)}{Exception}";
        }
    }
}
