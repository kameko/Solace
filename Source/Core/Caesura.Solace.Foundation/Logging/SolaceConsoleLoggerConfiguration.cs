
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    
    public class SolaceConsoleLoggerConfiguration
    {
        public LogLevel LogLevel                       { get; set; } = LogLevel.Debug;
        public int EventId                             { get; set; } = 0;
        public ConsoleColor ForegroundColor            { get; set; } = ConsoleColor.Blue;
        public ConsoleColor BackgroundColor            { get; set; } = ConsoleColor.Black;
        public string TimeStampFormat                  { get; set; } = "dd/MM/yyyy H:mm:ss.fff";
        public ISolaceConsoleLoggerFormatter Formatter { get; set; } = new SolaceConsoleLoggerFormatter();
        public CancellationToken Token                 { get; set; } = LifetimeEventsHostedService.Token;
    }
}
