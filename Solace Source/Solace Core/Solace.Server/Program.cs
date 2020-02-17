
namespace Solace.Server
{
    using System;
    using System.Threading.Tasks;
    using Core;
    using Serilog;
    
    class Program
    {
        static async Task Main(string[] args)
        {
            // TODO: parse args for:
            // - config path (default is null or "./solace.conf")
            
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            
            Core.Log.OnLog += SerilogLogger;
            
            var system = new SystemManager(null);
            await system.Setup();
            system.Start();
            
            Console.ReadLine();
            
            system.Stop();
            
            // Console.ReadLine();
        }
        
        private static void SerilogLogger(Core.Log.LogToken log)
        {
            var logger = Serilog.Log.Logger;
            logger.ForContext("CallerFilePath", log.CallerFilePath);
            logger.ForContext("CallerMemberName", log.CallerMemberName);
            logger.ForContext("CallerLineNumber", log.CallerLineNumber);
            
            if (log.Exception is null && log.Arguments is null)
            {
                switch (log.Level)
                {
                    case Core.Log.LogLevel.Write:
                    case Core.Log.LogLevel.Info:
                        logger.Information(log.Message);
                        break;
                    case Core.Log.LogLevel.Warning:
                        logger.Warning(log.Message);
                        break;
                    case Core.Log.LogLevel.Error:
                        logger.Error(log.Message);
                        break;
                    case Core.Log.LogLevel.Fatal:
                        logger.Fatal(log.Message);
                        break;
                    case Core.Log.LogLevel.Debug:
                        logger.Debug(log.Message);
                        break;
                    case Core.Log.LogLevel.Verbose:
                        logger.Verbose(log.Message);
                        break;
                }
            }
            else if (!(log.Exception is null) && log.Arguments is null)
            {
                switch (log.Level)
                {
                    case Core.Log.LogLevel.Write:
                    case Core.Log.LogLevel.Info:
                        logger.Information(log.Exception, log.Message);
                        break;
                    case Core.Log.LogLevel.Warning:
                        logger.Warning(log.Exception, log.Message);
                        break;
                    case Core.Log.LogLevel.Error:
                        logger.Error(log.Exception, log.Message);
                        break;
                    case Core.Log.LogLevel.Fatal:
                        logger.Fatal(log.Exception, log.Message);
                        break;
                    case Core.Log.LogLevel.Debug:
                        logger.Debug(log.Exception, log.Message);
                        break;
                    case Core.Log.LogLevel.Verbose:
                        logger.Verbose(log.Exception, log.Message);
                        break;
                }
            }
            else if (log.Exception is null && !(log.Arguments is null))
            {
                switch (log.Level)
                {
                    case Core.Log.LogLevel.Write:
                    case Core.Log.LogLevel.Info:
                        logger.Information(log.Message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Warning:
                        logger.Warning(log.Message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Error:
                        logger.Error(log.Message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Fatal:
                        logger.Fatal(log.Message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Debug:
                        logger.Debug(log.Message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Verbose:
                        logger.Verbose(log.Message, log.Arguments);
                        break;
                }
            }
            else
            {
                switch (log.Level)
                {
                    case Core.Log.LogLevel.Write:
                    case Core.Log.LogLevel.Info:
                        logger.Information(log.Exception, log.Message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Warning:
                        logger.Warning(log.Exception, log.Message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Error:
                        logger.Error(log.Exception, log.Message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Fatal:
                        logger.Fatal(log.Exception, log.Message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Debug:
                        logger.Debug(log.Exception, log.Message, log.Arguments);
                        break;
                    case Core.Log.LogLevel.Verbose:
                        logger.Verbose(log.Exception, log.Message, log.Arguments);
                        break;
                }
            }
        }
    }
}
