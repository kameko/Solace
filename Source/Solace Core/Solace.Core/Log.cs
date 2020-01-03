
namespace Solace.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using Serilog;
    using Serilog.Events;
    
    public static class Log
    {
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
        }
        
        private static void Write(LogEventLevel level, Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Write(level, ex, message);
        }
        
        private static void Write(LogEventLevel level, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Write(level, message, args);
        }
        
        private static void Write(LogEventLevel level, Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Write(level, ex, message, args);
        }
        
        public static void Info(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Information(message);
        }
        
        public static void Info(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Information(message, args);
        }
        
        public static void Info(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Information(ex, message);
        }
        
        public static void Info(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Information(ex, message, args);
        }
        
        public static void Warning(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Warning(message);
        }
        
        public static void Warning(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Warning(message, args);
        }
        
        public static void Warning(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Warning(ex, message);
        }
        
        public static void Warning(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Warning(ex, message, args);
        }
        
        public static void Error(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Error(message);
        }
        
        public static void Error(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Error(message, args);
        }
        
        public static void Error(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Error(ex, message);
        }
        
        public static void Error(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Error(ex, message, args);
        }
        
        public static void Fatal(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Fatal(message);
        }
        
        public static void Fatal(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Fatal(message, args);
        }
        
        public static void Fatal(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Fatal(ex, message);
        }
        
        public static void Fatal(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Fatal(ex, message, args);
        }
        
        public static void Debug(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Debug(message);
        }
        
        public static void Debug(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Debug(message, args);
        }
        
        public static void Debug(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Debug(ex, message);
        }
        
        public static void Debug(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Debug(ex, message, args);
        }
        
        public static void Verbose(string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Verbose(message);
        }
        
        public static void Verbose(string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Verbose(message, args);
        }
        
        public static void Verbose(Exception ex, string message, 
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Verbose(ex, message);
        }
        
        public static void Verbose(Exception ex, string message, object[] args,
            [CallerFilePath] string sourceFilePath = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logger = BuildContext(sourceFilePath, callerName, sourceLineNumber);
            logger.Verbose(ex, message, args);
        }
    }
}
