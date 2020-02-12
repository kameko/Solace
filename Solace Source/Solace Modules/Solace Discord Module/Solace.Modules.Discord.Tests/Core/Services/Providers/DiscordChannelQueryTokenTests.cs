
namespace Solace.Modules.Discord.Tests.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services.Providers;
    using Xunit;
    using Xunit.Abstractions;
    
    public class DiscordChannelQueryTokenTests
    {
        private readonly ITestOutputHelper output;

        public DiscordChannelQueryTokenTests(ITestOutputHelper output)
        {
            this.output = output;
            Write(string.Empty);
        }
        
        public void Write(string message)
        {
            output.WriteLine(message);
        }
    }
}
