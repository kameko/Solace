
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    
    public class SolaceConsoleLoggerConfiguration
    {
        public LogLevel LogLevel                        { get; set; } = LogLevel.Debug;
        public int EventId                              { get; set; } = 0;
        public string TimeStampFormat                   { get; set; } = "dddd H:mm:ss.fff"; // or "dd/MM/yyyy"
        public IEnumerable<string> TrimNames            { get; set; } = new List<string>() { "Caesura.Solace." };
        public IDictionary<string, string> ReplaceNames { get; set; } = new Dictionary<string, string>() { { "Microsoft.Hosting.Lifetime", "System" } };
        public ISolaceConsoleLoggerFormatter Formatter  { get; set; } = new SolaceConsoleLoggerFormatter();
        public CancellationToken Token                  { get; set; } = LifetimeEventsHostedService.Token;
        public IConsoleTheme Theme                      { get; set; } = Themes.Native;
        
        public SolaceConsoleLoggerConfiguration Clone()
        {
            return (SolaceConsoleLoggerConfiguration)MemberwiseClone();
        }
    }
}
