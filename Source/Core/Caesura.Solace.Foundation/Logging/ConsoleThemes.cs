
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    
    public static class Themes
    {
        public static readonly IConsoleTheme Native = new ConsoleTheme()
        {
            Name                     = "Native",
            Credit                   = "Caesura",
            InfoColor                = ConsoleColor.DarkCyan,
            WarnColor                = ConsoleColor.Yellow,
            ErrorColor               = ConsoleColor.Red,
            CriticalColor            = ConsoleColor.DarkRed,
            DebugColor               = ConsoleColor.Gray,
            TraceColor               = ConsoleColor.Gray,
            BracketColor             = ConsoleColor.DarkGray,
            TimeStampColor           = ConsoleColor.DarkGray,
            NameColor                = ConsoleColor.DarkGray,
            MessageColor             = ConsoleColor.Gray,
            ExceptionWarningColor    = ConsoleColor.Yellow,
            ExceptionMetaColor       = ConsoleColor.DarkRed,
            ExceptionNameColor       = ConsoleColor.DarkRed,
            ExceptionMessageColor    = ConsoleColor.Red,
            ExceptionStackTraceColor = ConsoleColor.Red,
        };
        
        public static readonly IConsoleTheme BlueAsMySoul = new ConsoleTheme()
        {
            Name                     = "BlueAsMySoul",
            Credit                   = "Caesura",
            InfoColor                = ConsoleColor.Blue,
            WarnColor                = ConsoleColor.Yellow,
            ErrorColor               = ConsoleColor.Red,
            CriticalColor            = ConsoleColor.DarkRed,
            DebugColor               = ConsoleColor.Gray,
            TraceColor               = ConsoleColor.Gray,
            BracketColor             = ConsoleColor.DarkGray,
            TimeStampColor           = ConsoleColor.Blue,
            NameColor                = ConsoleColor.Blue,
            MessageColor             = ConsoleColor.Blue,
            ExceptionWarningColor    = ConsoleColor.Yellow,
            ExceptionMetaColor       = ConsoleColor.DarkRed,
            ExceptionNameColor       = ConsoleColor.DarkRed,
            ExceptionMessageColor    = ConsoleColor.Red,
            ExceptionStackTraceColor = ConsoleColor.Red,
        };
    }
}
