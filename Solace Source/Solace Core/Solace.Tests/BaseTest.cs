
namespace Solace.Tests
{
    using System;
    using System.Linq;
    using Core;
    using Xunit.Abstractions;
    
    public class BaseTest
    {
        private readonly ITestOutputHelper output;
        
        public BaseTest(ITestOutputHelper output)
        {
            this.output = output;
            Write(string.Empty);
            Log.OnLog += OnLog;
        }
        
        protected void Write(string message)
        {
            output.WriteLine(message);
            Console.WriteLine(message);
        }
        
        private void OnLog(Log.LogToken message)
        {
            var file = message.CallerFilePath.Split('\\', '/').Last().Split('.').First();
            Write($"[{message.Level.ToString().ToUpper()}] {file}.{message.CallerMemberName}#{message.CallerLineNumber}: {message.Message}");
        }
    }
}
