
namespace Solace.Modules.Discord.Tests.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services.Providers;
    using Solace.Modules.Discord.Core;
    using Xunit;
    using Xunit.Abstractions;
    
    public class DiscordChannelQueryTokenTests : BaseTest
    {
        public DiscordChannelQueryTokenTests(ITestOutputHelper output) : base(output) { }
        
        private PseudoDiscordCache GenerateDefaultCache()
        {
            var pdc = new PseudoDiscordCache();
            
            var pdg  = new PseudoDiscordGuild();
            pdg.Name = "Guild1";
            pdg.Id   = 496217798167620000;
            pdc.Guilds.Add(pdg);
            
            for (int i = 0; i < 4; i++)
            {
                var c  = new PseudoDiscordChannel();
                c.Id   = 677026613363210000 + (ulong)i;
                c.Name = $"channel-{i}";
                pdg.Channels.Add(c);
                
                for (int j = 0; j < 1_001; j++)
                {
                    var m       = new SolaceDiscordMessage();
                    m.MessageId = 677069394039200000 + (ulong)j;
                    m.Created   = new DateTime(j + 1_000, 1, 1); // one message every year :)
                    m.Message   = $"Message number {j}";
                    // m.Sender    = new SolaceDiscordUser();
                }
            }
            
            return pdc;
        }
        
        [Fact]
        public async Task TestBaseFunctionality()
        {
            var cache = GenerateDefaultCache();
            var provider = new MockDiscordProvider(cache);
            
            var query = await provider.QueryChannel(677026613363210000 + 1);
            Assert.NotNull(query);
            
            var count = 3;
            await foreach (var message in query!)
            {
                if (count <= 0)
                {
                    break;
                }
                
                Write(message.Message);
                
                count--;
            }
        }
    }
}
