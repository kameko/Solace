
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    
    public class SolaceConsoleLoggerFormatter : ISolaceConsoleLoggerFormatter
    {
        private readonly ConsoleColor original_foreground_color;
        private readonly ConsoleColor original_background_color;
        private readonly List<string> errors;
        
        public SolaceConsoleLoggerFormatter()
        {
            Console.ResetColor();
            
            original_foreground_color = Console.ForegroundColor;
            original_background_color = Console.BackgroundColor;
            errors                    = new List<string>();
        }
        
        public void PreLog(LogItem item)
        {
            errors.Clear();
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
                MessageFormatter(item);
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
            
            ReportInternalErrors(item);
            
            if (newline)
            {
                WriteLine();
            }
            
            return item.ToString();
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
        
        private void MessageFormatter(LogItem item)
        {
            var original_foreground = Console.ForegroundColor;
            var original_background = Console.BackgroundColor;
            
            original_foreground     = Console.ForegroundColor;
            Console.ForegroundColor = item.Configuration.Theme.MessageColor;
            
            if (item.Configuration.StringifyOption == SolaceConsoleLoggerConfiguration.ObjectStringifyOption.CallToString)
            {
                WriteState();
            }
            else if (item.Configuration.StringifyOption == SolaceConsoleLoggerConfiguration.ObjectStringifyOption.SerializeJsonPretty)
            {
                WriteJson(indent: true);
            }
            else if (item.Configuration.StringifyOption == SolaceConsoleLoggerConfiguration.ObjectStringifyOption.SerializeJsonRaw)
            {
                WriteJson(indent: false);
            }
            else
            {
                errors.Add(
                    $"Unrecognized {nameof(SolaceConsoleLoggerConfiguration.ObjectStringifyOption)} "
                  + $"option: {item.Configuration.StringifyOption}."
                );
            }
            
            Console.ForegroundColor = original_foreground;
            
            void WriteState()
            {
                Write(item.State?.ToString() ?? string.Empty);
            }
            
            void WriteJson(bool indent)
            {
                if (item.State is SolaceLogState sls)
                {
                    var json = sls.ToJson(indent);
                    Write(json);
                }
                else
                {
                    WriteState();
                }
            }
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
        
        private void ReportInternalErrors(LogItem item)
        {
            if (errors.Count > 0)
            {
                var original_foreground = Console.ForegroundColor;
                var original_background = Console.BackgroundColor;
                
                WriteLine();
                Console.ForegroundColor = item.Configuration.Theme.ExceptionMessageColor;
                Write(
                    $"{nameof(SolaceConsoleLoggerFormatter)} Error: {errors.Count} "
                  + $"error{(errors.Count == 1 ? string.Empty : "s")} encountered."
                );
                
                var count = 1;
                foreach (var error in errors)
                {
                    WriteLine();
                    Write($" [{count}]: {error}");
                    count++;
                }
                
                Console.ForegroundColor = original_foreground;
            }
        }
        
        private void Write(object item)
        {
            var str = item.ToString();
            Write(str!);
        }
        
        private void Write(string str)
        {
            // builder.Append(str);
            Console.Write(str);
        }
        
        private void WriteLine()
        {
            // builder.AppendLine();
            Console.WriteLine();
        }
    }
}
