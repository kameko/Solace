
namespace Solace.Core
{
    using System;
    using System.Runtime.CompilerServices;
    
    using Serilog.Events;
    
    public static class Log
    {
        public static event Action<LogToken> OnLog;
        
        static Log()
        {
            OnLog = delegate { };
        }
        
        public enum LogLevel
        {
            Write,
            Info,
            Warning,
            Error,
            Fatal,
            Debug,
            Verbose,
        }
        
        public struct LogToken
        {
            public LogLevel Level { get; set; }
            public string CallerFilePath { get; set; }
            public string CallerMemberName { get; set; }
            public int CallerLineNumber { get; set; }
            public string Message { get; set; }
            public Exception? Exception { get; set; }
            public object[] Arguments { get; set; }
            
            public LogToken(LogLevel level, string path, string member, int line, string message)
            {
                Level            = level;
                CallerFilePath   = path;
                CallerMemberName = member;
                CallerLineNumber = line;
                Message          = message;
                Exception        = null;
                Arguments        = new object[0];
            }
            
            public LogToken(LogLevel level, string path, string member, int line, Exception exception, string message)
            {
                Level            = level;
                CallerFilePath   = path;
                CallerMemberName = member;
                CallerLineNumber = line;
                Message          = message;
                Exception        = exception;
                Arguments        = new object[0];
            }
            
            public LogToken(LogLevel level, string path, string member, int line, string message, object[] args)
            {
                Level            = level;
                CallerFilePath   = path;
                CallerMemberName = member;
                CallerLineNumber = line;
                Message          = message;
                Exception        = null;
                Arguments        = args;
            }
            
            public LogToken(LogLevel level, string path, string member, int line, Exception exception, string message, object[] args)
            {
                Level            = level;
                CallerFilePath   = path;
                CallerMemberName = member;
                CallerLineNumber = line;
                Message          = message;
                Exception        = exception;
                Arguments        = args;
            }
        }
        
        // TODO: remove Serilog dependency and move this into the Server project.
        public static void AttachSerilog()
        {
            OnLog += SerilogLogger;
        }
        
        private static void SerilogLogger(LogToken log)
        {
            var logger = Serilog.Log.Logger;
            logger.ForContext("CallerFilePath", log.CallerFilePath);
            logger.ForContext("CallerMemberName", log.CallerMemberName);
            logger.ForContext("CallerLineNumber", log.CallerLineNumber);
            
            if (log.Exception is null && log.Arguments.Length == 0)
            {
                switch (log.Level)
                {
                    case LogLevel.Write:
                    case LogLevel.Info:
                        logger.Information(log.Message);
                        break;
                    case LogLevel.Warning:
                        logger.Warning(log.Message);
                        break;
                    case LogLevel.Error:
                        logger.Error(log.Message);
                        break;
                    case LogLevel.Fatal:
                        logger.Fatal(log.Message);
                        break;
                    case LogLevel.Debug:
                        logger.Debug(log.Message);
                        break;
                    case LogLevel.Verbose:
                        logger.Verbose(log.Message);
                        break;
                }
            }
            else if (!(log.Exception is null) && log.Arguments.Length == 0)
            {
                switch (log.Level)
                {
                    case LogLevel.Write:
                    case LogLevel.Info:
                        logger.Information(log.Exception, log.Message);
                        break;
                    case LogLevel.Warning:
                        logger.Warning(log.Exception, log.Message);
                        break;
                    case LogLevel.Error:
                        logger.Error(log.Exception, log.Message);
                        break;
                    case LogLevel.Fatal:
                        logger.Fatal(log.Exception, log.Message);
                        break;
                    case LogLevel.Debug:
                        logger.Debug(log.Exception, log.Message);
                        break;
                    case LogLevel.Verbose:
                        logger.Verbose(log.Exception, log.Message);
                        break;
                }
            }
            else if (log.Exception is null && log.Arguments.Length > 0)
            {
                switch (log.Level)
                {
                    case LogLevel.Write:
                    case LogLevel.Info:
                        logger.Information(log.Message, log.Arguments);
                        break;
                    case LogLevel.Warning:
                        logger.Warning(log.Message, log.Arguments);
                        break;
                    case LogLevel.Error:
                        logger.Error(log.Message, log.Arguments);
                        break;
                    case LogLevel.Fatal:
                        logger.Fatal(log.Message, log.Arguments);
                        break;
                    case LogLevel.Debug:
                        logger.Debug(log.Message, log.Arguments);
                        break;
                    case LogLevel.Verbose:
                        logger.Verbose(log.Message, log.Arguments);
                        break;
                }
            }
            else
            {
                switch (log.Level)
                {
                    case LogLevel.Write:
                    case LogLevel.Info:
                        logger.Information(log.Exception, log.Message, log.Arguments);
                        break;
                    case LogLevel.Warning:
                        logger.Warning(log.Exception, log.Message, log.Arguments);
                        break;
                    case LogLevel.Error:
                        logger.Error(log.Exception, log.Message, log.Arguments);
                        break;
                    case LogLevel.Fatal:
                        logger.Fatal(log.Exception, log.Message, log.Arguments);
                        break;
                    case LogLevel.Debug:
                        logger.Debug(log.Exception, log.Message, log.Arguments);
                        break;
                    case LogLevel.Verbose:
                        logger.Verbose(log.Exception, log.Message, log.Arguments);
                        break;
                }
            }
        }
        
        private static void Write(LogEventLevel level, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        private static void Write(LogEventLevel level, Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        private static void Write(LogEventLevel level, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        private static void Write(LogEventLevel level, Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Info(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Info, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Info(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Info, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Info(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Info, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Info(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Info, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Warning(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Warning, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Warning(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Warning, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Warning(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Warning, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Warning(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Warning, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Error(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Error, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Error(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Error, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Error(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Error, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Error(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Error, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Fatal(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Fatal, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Fatal(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Fatal, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Fatal(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Fatal, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Fatal(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Fatal, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Debug(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Debug, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Debug(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Debug, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Debug(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Debug, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Debug(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Debug, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Verbose(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Verbose, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Verbose(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Verbose, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Verbose(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Verbose, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Verbose(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Verbose, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
    }
}
