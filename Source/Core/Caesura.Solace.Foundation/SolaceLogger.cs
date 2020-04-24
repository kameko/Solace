
namespace Caesura.Solace.Foundation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    
    public class SolaceLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; }  = LogLevel.Warning;
        public int EventId { get; set; }        = 0;
        public ConsoleColor Color { get; set; } = ConsoleColor.Blue;
    }
    
    public class SolaceLoggerProvider : ILoggerProvider
    {
        private readonly SolaceLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, SolaceLogger> _loggers = new ConcurrentDictionary<string, SolaceLogger>();
        
        public SolaceLoggerProvider(SolaceLoggerConfiguration config)
        {
            _config = config;
        }
        
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new SolaceLogger(name, _config));
        }
        
        public void Dispose()
        {
            _loggers.Clear();
        }
    }
    
    public class SolaceLogger : ILogger
    {
        private static object _lock = new Object();
        
        private readonly string _name;
        private readonly SolaceLoggerConfiguration _config;
        
        public SolaceLogger(string name, SolaceLoggerConfiguration config)
        {
            _name   = name;
            _config = config;
        }
        
        public IDisposable? BeginScope<TState>(TState state)
        {
            return null;
        }
        
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == _config.LogLevel;
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            lock (_lock)
            {
                if (_config.EventId == 0 || _config.EventId == eventId.Id)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = _config.Color;
                    Console.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {_name} - {formatter(state, exception)}");
                    Console.ForegroundColor = color;
                }
            }
        }
    }
}
