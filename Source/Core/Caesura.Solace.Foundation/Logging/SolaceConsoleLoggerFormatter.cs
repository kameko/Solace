
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    
    public class SolaceConsoleLoggerFormatter : ISolaceConsoleLoggerFormatter
    {
        private readonly ConsoleColor original_foreground_color;
        private readonly ConsoleColor original_background_color;
        private readonly StringBuilder builder;
        
        public SolaceConsoleLoggerFormatter()
        {
            Console.ResetColor();
            
            original_foreground_color = Console.ForegroundColor;
            original_background_color = Console.BackgroundColor;
            builder                   = new StringBuilder();
        }
        
        public void PreLog(LogItem item)
        {
            builder.Clear();
        }
        
        public void PostLog(LogItem item)
        {
            Console.ForegroundColor = original_foreground_color;
            Console.BackgroundColor = original_background_color;
        }
        
        public string Format(LogItem item)
        {
            var message = item.State?.ToString() ?? string.Empty;
            var newline = true;
            
            var original_foreground = original_foreground_color;
            var original_background = original_background_color;
            
            if (message.Contains("<$NoStamp>"))
            {
                message = message.Replace("<$NoStamp>", string.Empty);
            }
            else
            {
                StampFormatter(item);
            }
            
            if (message.Contains("<$NoNewLine>"))
            {
                message = message.Replace("<$NoNewLine>", string.Empty);
                newline = false;
            }
            
            if (!string.IsNullOrEmpty(message))
            {
                original_foreground     = Console.ForegroundColor;
                Console.ForegroundColor = item.Configuration.Theme.MessageColor;
                Write(message);
                Console.ForegroundColor = original_foreground;
            }
            
            if (!(item.Exception is null))
            {
                if (string.IsNullOrEmpty(message))
                {
                    Write(" ");
                }
                else
                {
                    WriteLine();
                }
                
                ExceptionFormatter(item);
            }
            
            if (newline)
            {
                WriteLine();
            }
            
            return builder.ToString();
        }
        
        private void StampFormatter(LogItem item)
        {
            var original_foreground = Console.ForegroundColor;
            var original_background = Console.BackgroundColor;
            
            var level_color = item.Level switch
            {
                LogLevel.Information => item.Configuration.Theme.InfoColor,
                LogLevel.Warning     => item.Configuration.Theme.WarnColor,
                LogLevel.Error       => item.Configuration.Theme.ErrorColor,
                LogLevel.Critical    => item.Configuration.Theme.CriticalColor,
                LogLevel.Debug       => item.Configuration.Theme.DebugColor,
                LogLevel.Trace       => item.Configuration.Theme.TraceColor,
                LogLevel.None        => ConsoleColor.Gray,
                
                _ => ConsoleColor.Gray
            };
            
            Console.ForegroundColor = item.Configuration.Theme.BracketColor;
            Write("[");
            Console.ForegroundColor = level_color;
            Write(item.Level);
            Console.ForegroundColor = item.Configuration.Theme.BracketColor;
            Write("]");
            Console.ForegroundColor = original_foreground;
            
            Console.ForegroundColor = item.Configuration.Theme.BracketColor;
            Write("[");
            Console.ForegroundColor = item.Configuration.Theme.TimeStampColor;
            Write(item.TimeStamp.ToString(item.Configuration.TimeStampFormat));
            Console.ForegroundColor = item.Configuration.Theme.BracketColor;
            Write("]");
            Console.ForegroundColor = original_foreground;
            
            var name = item.Name;
            foreach (var trim in item.Configuration.TrimNames)
            {
                if (name.StartsWith(trim))
                {
                    name = name.Replace(trim, string.Empty);
                    break;
                }
            }
            foreach (var (val, repl) in item.Configuration.ReplaceNames)
            {
                if (name == val)
                {
                    name = repl;
                    break;
                }
            }
            
            if (!string.IsNullOrEmpty(name))
            {
                Console.ForegroundColor = item.Configuration.Theme.BracketColor;
                Write("[");
                Console.ForegroundColor = item.Configuration.Theme.NameColor;
                Write(name);
                if (item.Id != 0)
                {
                    Write("(");
                    Write(item.Id);
                    Write(")");
                }
                Console.ForegroundColor = item.Configuration.Theme.BracketColor;
                Write("]");
                Console.ForegroundColor = original_foreground;
            }
            
            Write(" ");
        }
        
        private void ExceptionFormatter(LogItem item)
        {
            var exception = item.Exception!;
            
            var original_foreground = Console.ForegroundColor;
            var original_background = Console.BackgroundColor;
            
            Console.ForegroundColor = item.Configuration.Theme.ExceptionWarningColor;
            Write("EXCEPTION: ");
            Console.ForegroundColor = item.Configuration.Theme.ExceptionNameColor;
            Write(exception.GetType().FullName ?? "<NO TYPE NAME>");
            WriteLine();
            
            if (!string.IsNullOrEmpty(exception.Message))
            {
                Console.ForegroundColor = item.Configuration.Theme.ExceptionMetaColor;
                Write("MESSAGE: ");
                Console.ForegroundColor = item.Configuration.Theme.ExceptionMessageColor;
                Write(exception.Message);
                WriteLine();
            }
            
            Console.ForegroundColor = item.Configuration.Theme.ExceptionMetaColor;
            Write(" Stack Trace: ");
            WriteLine();
            Console.ForegroundColor = item.Configuration.Theme.ExceptionStackTraceColor;
            Write(exception.StackTrace ?? " <NO STACK TRACE>");
            Console.ForegroundColor = item.Configuration.Theme.ExceptionMetaColor;
            WriteLine();
            Write(" --- End of stack trace ---");
            
            Console.ForegroundColor = original_foreground;
        }
        
        private void Write(object item)
        {
            var str = item.ToString();
            Write(str!);
        }
        
        private void Write(string str)
        {
            builder.Append(str);
            Console.Write(str);
        }
        
        private void WriteLine()
        {
            builder.AppendLine();
            Console.WriteLine();
        }
    }
}
