
namespace Solace.Core
{
    using System;
    using System.Runtime.CompilerServices;
    
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
            public object[]? Arguments { get; set; }
            
            public LogToken(LogLevel level, string path, string member, int line, string message)
            {
                Level            = level;
                CallerFilePath   = path;
                CallerMemberName = member;
                CallerLineNumber = line;
                Message          = message;
                Exception        = null;
                Arguments        = null;
            }
            
            public LogToken(LogLevel level, string path, string member, int line, Exception exception, string message)
            {
                Level            = level;
                CallerFilePath   = path;
                CallerMemberName = member;
                CallerLineNumber = line;
                Message          = message;
                Exception        = exception;
                Arguments        = null;
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
        
        public static void Write(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, message));
        }
        
        public static void Write(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, ex, message));
        }
        
        public static void Write(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            OnLog.Invoke(new LogToken(LogLevel.Write, sourceFilePath, callerName, sourceLineNumber, message, args));
        }
        
        public static void Write(Exception ex, string message, object[] args,
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
