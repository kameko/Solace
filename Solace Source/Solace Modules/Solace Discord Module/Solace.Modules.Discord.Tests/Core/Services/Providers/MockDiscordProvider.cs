
namespace Solace.Modules.Discord.Tests.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Solace.Modules.Discord.Core;
    using Solace.Modules.Discord.Core.Services.Providers;
    
    public class MockDiscordProvider : BaseDiscordProvider, IDiscordProvider
    {
        private PseudoDiscordCache Cache { get; set; }
        
        public MockDiscordProvider(PseudoDiscordCache cache) : base()
        {
            Cache            = cache;
            MaxQueryLimit    = 100;
        }
        
        public override async Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id, ulong starting_message_id)
        {
            var guild = Cache.Guilds.Find(x => x.Channels.Exists(y => y.Id == channel_id));
            if (!(guild is null))
            {
                var channel = guild.Channels.Find(x => x.Id == channel_id);
                if (!(channel is null))
                {
                    var dcqt = new DiscordChannelQueryToken(this, channel_id, starting_message_id);
                    await dcqt.Setup();
                    return dcqt;
                }
            }
            return null;
        }
        
        public override async Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id)
        {
            var guild = Cache.Guilds.Find(x => x.Channels.Exists(y => y.Id == channel_id));
            if (!(guild is null))
            {
                var channel = guild.Channels.Find(x => x.Id == channel_id);
                if (!(channel is null))
                {
                    var first = channel.Messages.FirstOrDefault();
                    if (!(first is null))
                    {
                        var dcqt = new DiscordChannelQueryToken(this, channel_id, first.MessageId);
                        await dcqt.Setup();
                        return dcqt;
                    }
                }
            }
            return null;
        }
        
        public override Task<SolaceDiscordMessage?> GetMessage(ulong channel_id, ulong message_id)
        {
            var guild = Cache.Guilds.Find(x => x.Channels.Exists(y => y.Id == channel_id));
            if (!(guild is null))
            {
                var channel = guild.Channels.Find(x => x.Id == channel_id);
                if (!(channel is null))
                {
                    var message = channel.Messages.Find(x => x.MessageId == message_id);
                    return Task.FromResult(message);
                }
            }
            return Task.FromResult<SolaceDiscordMessage?>(null);
        }
        
        public override Task<IEnumerable<SolaceDiscordMessage>?> QueryBefore(ulong channel_id, ulong before_message_id, int limit)
        {
            var guild = Cache.Guilds.Find(x => x.Channels.Exists(y => y.Id == channel_id));
            if (!(guild is null))
            {
                var channel = guild.Channels.Find(x => x.Id == channel_id);
                if (!(channel is null))
                {
                    var message = channel.Messages.Find(x => x.MessageId == before_message_id);
                    if (!(message is null))
                    {
                        var list = new List<SolaceDiscordMessage>(limit);
                        
                        int index = channel.Messages.IndexOf(message) + 1;
                        while (limit > 0 && index < channel.Messages.Count)
                        {
                            var current = channel.Messages.ElementAt(index);
                            limit--;
                            index++;
                            list.Add(current);
                        }
                        
                        return Task.FromResult(list as IEnumerable<SolaceDiscordMessage?>)!;
                    }
                }
            }
            return Task.FromResult<IEnumerable<SolaceDiscordMessage>?>(null);
        }
        
        public override async Task<bool> Connect()
        {
            await Task.Delay(1000); // Simulate logging in.
            await RaiseOnReady();
            throw new NotImplementedException();
        }
    }
}
