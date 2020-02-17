
namespace Solace.Server
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Serilog;
    
    class Program
    {
        static async Task Main(string[] args)
        {
            // TODO: parse args for:
            // - config path (default is null or "./solace.conf")
            
            var template = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] "
                         + "{CallerMemberName}@{CallerFileName}#{CallerLineNumber}: "
                         + "{Message:lj}{NewLine}{Exception}";
            
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: template)
                .CreateLogger();
            
            Core.Log.OnLog += SerilogLogger;
            
            Core.Log.Info("Starting Solice");
            
            var system = new SystemManager(null);
            await system.Setup();
            system.Start();
            
            Console.ReadLine();
            
            system.Stop();
            
            // Console.ReadLine();
        }
        
        private static void SerilogLogger(Core.Log.LogToken log)
        {
            var callerfile = log.CallerFilePath.Split('\\', '/').ToList().LastOrDefault() ?? string.Empty;
            
            var logger = Serilog.Log.Logger;
            logger = logger.ForContext("CallerFileName"  , callerfile);
            logger = logger.ForContext("CallerFilePath"  , log.CallerFilePath);
            logger = logger.ForContext("CallerMemberName", log.CallerMemberName);
            logger = logger.ForContext("CallerLineNumber", log.CallerLineNumber);
            
            var message = log.Message;
            
            if (log.Exception is null && log.Arguments is null)
            {
                switch (log.Level)
                {
                    case Core.Log.LogLevel.Write:
                    case Core.Log.LogLevel.Info:
                        logger.Information(message);
                        break;
                    case Core.Log.LogLevel.Warning:
                        logger.Warning(message);
                        break;
                    case Core.Log.LogLevel.Error:
                        logger.Error(message);
                        break;
                    case Core.Log.LogLevel.Fatal:
                        logger.Fatal(message);
                        break;
                    case Core.Log.LogLevel.Debug:
                        logger.Debug(message);
                        break;
                    case Core.Log.LogLevel.Verbose:
                        logger.Verbose(message);
                        break;
                }
            }
            else if (!(log.Exception is null) && log.Arguments is null)
            {
                switch (log.Level)
                {
                    case Core.Log.LogLevel.Write:
                    case Core.Log.LogLevel.Info:
                        logger.Information(log.Exception, message);
                        break;
                    case Core.Log.LogLevel.Warning:
                        logger.Warning(log.Exception, message);
                        break;
                    case Core.Log.LogLevel.Error:
                        logger.Error(log.Exception, message);
                        break;
                    case Core.Log.LogLevel.Fatal:
                        logger.Fatal(log.Exception, message);
                        break;
                    case Core.Log.LogLevel.Debug:
                        logger.Debug(log.Exception, message);
                        break;
                    case Core.Log.LogLevel.Verbose:
                        logger.Verbose(log.Exception, message);
                        break;
                }
            }
            else if (log.Exception is null && !(log.Arguments is null))
            {
                switch (log.Level)
                {
                    case Core.Log.LogLevel.Write:
                    case Core.Log.LogLevel.Info:
                        logger.Information(message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Warning:
                        logger.Warning(message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Error:
                        logger.Error(message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Fatal:
                        logger.Fatal(message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Debug:
                        logger.Debug(message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Verbose:
                        logger.Verbose(message, log.Arguments);
                        break;
                }
            }
            else
            {
                switch (log.Level)
                {
                    case Core.Log.LogLevel.Write:
                    case Core.Log.LogLevel.Info:
                        logger.Information(log.Exception, message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Warning:
                        logger.Warning(log.Exception, message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Error:
                        logger.Error(log.Exception, message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Fatal:
                        logger.Fatal(log.Exception, message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Debug:
                        logger.Debug(log.Exception, message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Verbose:
                        logger.Verbose(log.Exception, message, log.Arguments);
                        break;
                }
            }
        }
    }
}
