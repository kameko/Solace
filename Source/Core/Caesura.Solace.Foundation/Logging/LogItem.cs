
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    using Entities.Core;
    
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
        
        public LogElement ToLogElement()
        {
            var le = new LogElement()
            {
                TimeStamp = this.TimeStamp,
                Level     = this.Level,
                Id        = this.Id.Id,
                Name      = this.Name,
                Message   = this.State?.ToString() ?? string.Empty,
                Exception = this.Exception,
            };
            return le;
        }
        
        public static implicit operator LogElement (LogItem item)
        {
            return item.ToLogElement();
        }
        
        public override string ToString()
        {
            return 
                $"[{Level}]({Id}): {State}{(State is null ? string.Empty : " ")} " + 
                $"{(Exception is null ? string.Empty : Environment.NewLine)}{Exception}";
        }
    }
}
