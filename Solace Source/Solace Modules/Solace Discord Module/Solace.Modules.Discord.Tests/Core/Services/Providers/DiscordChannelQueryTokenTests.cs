
namespace Solace.Modules.Discord.Tests.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services.Providers;
    using Xunit;
    using Xunit.Abstractions;
    
    public class DiscordChannelQueryTokenTests : BaseTest
    {
        public DiscordChannelQueryTokenTests(ITestOutputHelper output) : base(output) { }
        
        [Fact]
        public void Test1()
        {
            Solace.Core.Log.Info("Hello, world!");
        }
    }
}
