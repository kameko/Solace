
namespace Solace.Modules.Discord.Tests
{
    using System;
    using Xunit.Abstractions;
    
    public class BaseTest
    {
        private readonly ITestOutputHelper output;
        
        public BaseTest(ITestOutputHelper output)
        {
            this.output = output;
            Write(string.Empty);
        }
        
        protected void Write(string message)
        {
            output.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
