
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
            
            if (message.Contains("<$NoStamp>"))
            {
                message = message.Replace("<$NoStamp>", string.Empty);
            }
            else
            {
                StampFormatter(item);
            }
            
            if (!string.IsNullOrEmpty(message))
            {
                builder.Append(message);
            }
            
            if (!(item.Exception is null))
            {
                if (string.IsNullOrEmpty(message))
                {
                    builder.Append(" ");
                }
                else
                {
                    builder.AppendLine();
                }
                
                ExceptionFormatter(item);
            }
            
            return builder.ToString();
        }
        
        private void StampFormatter(LogItem item)
        {
            var original_foreground = item.Configuration.ForegroundColor;
            var original_background = item.Configuration.BackgroundColor;
            
            var level_color = item.Level switch
            {
                LogLevel.Information => ConsoleColor.Gray, //original_foreground,
                LogLevel.Warning     => ConsoleColor.Yellow,
                LogLevel.Error       => ConsoleColor.Red,
                LogLevel.Critical    => ConsoleColor.DarkRed,
                LogLevel.Debug       => ConsoleColor.DarkYellow,
                LogLevel.Trace       => ConsoleColor.DarkYellow,
                LogLevel.None        => ConsoleColor.Gray,
                
                _ => ConsoleColor.Gray
            };
            
            Console.ForegroundColor = level_color;
            
            builder.Append("[");
            builder.Append(item.Level);
            builder.Append("]");
            
            Console.ForegroundColor = ConsoleColor.Gray;
            
            builder.Append("[");
            builder.Append(item.TimeStamp.ToString(item.Configuration.TimeStampFormat));
            builder.Append("]");
            
            builder.Append("[");
            builder.Append(item.Name);
            if (item.Id != 0)
            {
                builder.Append("(");
                builder.Append(item.Id);
                builder.Append(")");
            }
            builder.Append("]");
            
            Console.ForegroundColor = original_foreground;
            
            builder.Append(" ");
        }
        
        private void ExceptionFormatter(LogItem item)
        {
            var exception = item.Exception!;
            
            var original_foreground = Console.ForegroundColor;
            var original_background = Console.BackgroundColor;
            
            Console.ForegroundColor = ConsoleColor.Red;
            
            builder.Append("EXCEPTION: ");
            builder.Append(exception.GetType().FullName);
            builder.AppendLine();
            
            if (!string.IsNullOrEmpty(exception.Message))
            {
                builder.Append("MESSAGE: ");
                builder.Append(exception.Message);
                builder.AppendLine();
            }
            
            builder.Append("Stack Trace: ");
            builder.AppendLine();
            builder.Append(exception.StackTrace);
            builder.AppendLine();
            builder.Append("--- End of stack trace ---");
            
            Console.ForegroundColor = original_foreground;
        }
    }
}
