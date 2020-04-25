
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    
    public static class SolaceLoggerExtensions
    {
        public static ILoggingBuilder AddSolaceConsoleLogger(this ILoggingBuilder builder)
        {
            AddSolaceConsoleLogger(builder, null);
            return builder;
        }
        
        public static ILoggingBuilder AddSolaceConsoleLogger(this ILoggingBuilder builder, LogLevel log_level)
        {
            var config = new SolaceConsoleLoggerConfiguration()
            {
                LogLevel = log_level,
            };
            
            var provider = new SolaceConsoleLoggerProvider(config);
            builder.AddProvider(provider);
            
            return builder;
        }
        
        public static ILoggingBuilder AddSolaceConsoleLogger(this ILoggingBuilder builder, Action<SolaceConsoleLoggerConfiguration>? configure)
        {
            var config = new SolaceConsoleLoggerConfiguration();
            configure?.Invoke(config);
            
            var provider = new SolaceConsoleLoggerProvider(config);
            builder.AddProvider(provider);
            
            return builder;
        }
    }
}
