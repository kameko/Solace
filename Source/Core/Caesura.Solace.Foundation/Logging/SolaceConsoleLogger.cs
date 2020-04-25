
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    
    public class SolaceConsoleLogger : ILogger
    {
        private static object console_lock = new Object();
        private static SolaceConsoleLoggerConfiguration? static_config;
        private static SolaceConsoleLoggerConfiguration? replacement_static_config;
        private static ConcurrentQueue<LogItem> queue;
        
        private readonly string _name;
        private readonly SolaceConsoleLoggerConfiguration _config;
        
        static SolaceConsoleLogger()
        {
            queue = new ConcurrentQueue<LogItem>();
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
                var item = new LogItem(_config, logLevel, eventId, _name, (state as object), exception);
                queue.Enqueue(item);
            }
        }
        
        public static void ChangeLogLevel(LogLevel level)
        {
            lock (console_lock)
            {
                if (static_config is null)
                {
                    return;
                }
                
                var conf                  = static_config.Clone();
                conf.LogLevel             = level;
                replacement_static_config = conf;
            }
        }
        
        public static void Reconfigure(SolaceConsoleLoggerConfiguration config)
        {
            lock (console_lock)
            {
                replacement_static_config = config;
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
                    while (queue.IsEmpty)
                    {
                        await Task.Delay(15);
                        continue;
                    }
                    
                    var success = queue.TryDequeue(out n_item);
                    if (!success || n_item is null)
                    {
                        continue;
                    }
                    
                    lock (console_lock)
                    {
                        if (!(replacement_static_config is null))
                        {
                            config = replacement_static_config;
                            replacement_static_config = null;
                        }
                        
                        var item = n_item;
                        n_item   = null;
                        
                        config.Formatter.PreLog(item);
                        config.Formatter.Format(item);
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
