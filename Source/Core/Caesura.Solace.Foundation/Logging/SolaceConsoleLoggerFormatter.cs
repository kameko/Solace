
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
            original_foreground_color = Console.ForegroundColor;
            original_background_color = Console.BackgroundColor;
            builder                   = new StringBuilder();
        }
        
        public void PreLog(LogItem item)
        {
            Console.ForegroundColor = item.Configuration.ForegroundColor;
            Console.BackgroundColor = item.Configuration.BackgroundColor;
            
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
                Write(message);
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
            var original_foreground = item.Configuration.ForegroundColor;
            var original_background = item.Configuration.BackgroundColor;
            
            var level_color = item.Level switch
            {
                LogLevel.Information => original_foreground,
                LogLevel.Warning     => ConsoleColor.Yellow,
                LogLevel.Error       => ConsoleColor.Red,
                LogLevel.Critical    => ConsoleColor.DarkRed,
                LogLevel.Debug       => ConsoleColor.DarkYellow,
                LogLevel.Trace       => ConsoleColor.DarkYellow,
                LogLevel.None        => ConsoleColor.Gray,
                
                _ => ConsoleColor.Gray
            };
            
            Console.ForegroundColor = level_color;
            
            Write("[");
            Write(item.Level);
            Write("]");
            
            Console.ForegroundColor = ConsoleColor.Gray;
            
            Write("[");
            Write(item.TimeStamp.ToString(item.Configuration.TimeStampFormat));
            Write("]");
            
            Write("[");
            Write(item.Name);
            if (item.Id != 0)
            {
                Write("(");
                Write(item.Id);
                Write(")");
            }
            Write("]");
            
            Console.ForegroundColor = original_foreground;
            
            Write(" ");
        }
        
        private void ExceptionFormatter(LogItem item)
        {
            var exception = item.Exception!;
            
            var original_foreground = Console.ForegroundColor;
            var original_background = Console.BackgroundColor;
            
            Console.ForegroundColor = ConsoleColor.Red;
            
            Write("EXCEPTION: ");
            Write(exception.GetType().FullName ?? "<NO TYPE NAME>");
            WriteLine();
            
            if (!string.IsNullOrEmpty(exception.Message))
            {
                Write("MESSAGE: ");
                Write(exception.Message);
                WriteLine();
            }
            
            Write("Stack Trace: ");
            WriteLine();
            Write(exception.StackTrace ?? "<NO STACK TRACE>");
            WriteLine();
            Write("--- End of stack trace ---");
            
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
