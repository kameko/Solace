
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    
    public class SolaceConsoleLogger : ILogger
    {
        private static object queue_lock   = new Object();
        private static object console_lock = new Object();
        private static SolaceConsoleLoggerConfiguration? static_config;
        private static Queue<LogItem> messages;
        
        private readonly string _name;
        private readonly SolaceConsoleLoggerConfiguration _config;
        
        static SolaceConsoleLogger()
        {
            messages = new Queue<LogItem>();
            Task.Run(LogHandler);
        }
        
        public SolaceConsoleLogger(string name, SolaceConsoleLoggerConfiguration config)
        {
            _name   = name;
            _config = config;
            static_config ??= config;
        }
        
        public IDisposable? BeginScope<TState>(TState state)
        {
            return null;
        }
        
        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Debug && !(_config.LogLevel == LogLevel.Debug || _config.LogLevel == LogLevel.Trace))
            {
                return false;
            }
            
            if (logLevel == LogLevel.Trace && _config.LogLevel != LogLevel.Trace)
            {
                return false;
            }
            
            return true;
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            
            if (_config.EventId == 0 || _config.EventId == eventId.Id)
            {
                lock (queue_lock)
                {
                    var item = new LogItem(_config, logLevel, eventId, _name, (state as object), exception);
                    messages.Enqueue(item);
                }
            }
        }
        
        private static async Task LogHandler()
        {
            LogItem? n_item = null;
            
            while (static_config is null)
            {
                await Task.Delay(15);
            }
            
            var config = static_config;
            while (!config.Token.IsCancellationRequested)
            {
                try
                {
                    lock (queue_lock)
                    {
                        messages.TryDequeue(out n_item);
                    }
                    
                    if (n_item is null)
                    {
                        await Task.Delay(15);
                        continue;
                    }
                    
                    lock (console_lock)
                    {
                        var item = n_item;
                        n_item   = null;
                        
                        config.Formatter.PreLog(item);
                        
                        var message = config.Formatter.Format(item);
                        if (item.State?.ToString()?.Contains("<$NoNewLine>") ?? false)
                        {
                            message = message.Replace("<$NoNewLine>", string.Empty);
                            Console.Write(message);
                        }
                        else
                        {
                            Console.WriteLine(message);
                        }
                        
                        config.Formatter.PostLog(item);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR IN SOLACE CONSOLE LOGGER: {e}");
                }
            }
            
            Console.ResetColor();
            Console.WriteLine("Stopping logger.");
        }
    }
}
