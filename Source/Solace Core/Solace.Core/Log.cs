
namespace Solace.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using Serilog;
    using Serilog.Events;
    
    // TODO: remove Serilog from this, put it elsewhere
    
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
        
        private static ILogger BuildContext(string sourceFilePath, string callerName, int sourceLineNumber)
        {
            var logger = Serilog.Log.Logger;
            logger.ForContext("CallerFilePath", sourceFilePath);
            logger.ForContext("CallerMemberName", callerName);
            logger.ForContext("CallerLineNumber", sourceLineNumber);
            return logger;
        }
        
        private static void Write(LogEventLevel level, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Write(level, message);
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        private static void Write(LogEventLevel level, Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Write(level, ex, message);
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        private static void Write(LogEventLevel level, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Write(level, message, args);
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        private static void Write(LogEventLevel level, Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Write(level, ex, message, args);
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Info(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Information(message);
            OnLog.Invoke(new LogToken(LogLevel.Info, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Info(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Information(message, args);
            OnLog.Invoke(new LogToken(LogLevel.Info, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Info(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Information(ex, message);
            OnLog.Invoke(new LogToken(LogLevel.Info, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Info(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Information(ex, message, args);
            OnLog.Invoke(new LogToken(LogLevel.Info, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Warning(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Warning(message);
            OnLog.Invoke(new LogToken(LogLevel.Warning, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Warning(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Warning(message, args);
            OnLog.Invoke(new LogToken(LogLevel.Warning, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Warning(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Warning(ex, message);
            OnLog.Invoke(new LogToken(LogLevel.Warning, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Warning(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Warning(ex, message, args);
            OnLog.Invoke(new LogToken(LogLevel.Warning, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Error(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Error(message);
            OnLog.Invoke(new LogToken(LogLevel.Error, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Error(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Error(message, args);
            OnLog.Invoke(new LogToken(LogLevel.Error, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Error(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Error(ex, message);
            OnLog.Invoke(new LogToken(LogLevel.Error, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Error(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Error(ex, message, args);
            OnLog.Invoke(new LogToken(LogLevel.Error, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Fatal(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Fatal(message);
            OnLog.Invoke(new LogToken(LogLevel.Fatal, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Fatal(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Fatal(message, args);
            OnLog.Invoke(new LogToken(LogLevel.Fatal, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Fatal(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Fatal(ex, message);
            OnLog.Invoke(new LogToken(LogLevel.Fatal, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Fatal(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Fatal(ex, message, args);
            OnLog.Invoke(new LogToken(LogLevel.Fatal, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Debug(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Debug(message);
            OnLog.Invoke(new LogToken(LogLevel.Debug, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Debug(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Debug(message, args);
            OnLog.Invoke(new LogToken(LogLevel.Debug, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Debug(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Debug(ex, message);
            OnLog.Invoke(new LogToken(LogLevel.Debug, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Debug(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Debug(ex, message, args);
            OnLog.Invoke(new LogToken(LogLevel.Debug, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
        
        public static void Verbose(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Verbose(message);
            OnLog.Invoke(new LogToken(LogLevel.Verbose, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Verbose(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Verbose(message, args);
            OnLog.Invoke(new LogToken(LogLevel.Verbose, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Verbose(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Verbose(ex, message);
            OnLog.Invoke(new LogToken(LogLevel.Verbose, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Verbose(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Verbose(ex, message, args);
            OnLog.Invoke(new LogToken(LogLevel.Verbose, sourceFilePath, callerName, sourceLineNumber, ex, message, args));
        }
    }
}
