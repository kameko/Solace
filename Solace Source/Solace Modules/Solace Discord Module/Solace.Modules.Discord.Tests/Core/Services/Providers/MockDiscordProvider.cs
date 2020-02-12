
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
    
    public class MockDiscordProvider : BaseProvider, IDiscordProvider
    {
        private PseudoDiscordCache Cache { get; set; }
        
        public bool Connected { get; protected set; }
        public int MaxQueryLimit { get; protected set; }
        public event Func<Task> OnReady;
        public event Func<SolaceDiscordMessage, Task> OnReceiveMessage;
        
        public MockDiscordProvider(PseudoDiscordCache cache)
        {
            Cache            = cache;
            MaxQueryLimit    = 100;
            OnReady          = delegate { return Task.CompletedTask; };
            OnReceiveMessage = delegate { return Task.CompletedTask; };
        }
        
        public async Task RaiseOnReceiveMessage(SolaceDiscordMessage message)
        {
            await OnReceiveMessage.Invoke(message);
        }
        
        public async Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id, ulong starting_message_id)
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
        
        public async Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id)
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
        
        public Task<SolaceDiscordMessage?> GetMessage(ulong channel_id, ulong message_id)
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
        
        public Task<SolaceDiscordMessage?> QueryLatest(ulong channel_id)
        {
            throw new NotImplementedException();
        }
        
        public Task<IEnumerable<SolaceDiscordMessage>?> QueryLatest(ulong channel_id, int limit)
        {
            throw new NotImplementedException();
        }
        
        public Task<IEnumerable<SolaceDiscordMessage>?> QueryBefore(ulong channel_id, ulong before_message_id, int limit)
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
        
        public Task<IEnumerable<SolaceDiscordMessage>?> QueryAfter(ulong channel_id, ulong before_message_id, int limit)
        {
            throw new NotImplementedException();
        }
        
        public Task<bool> Send(ulong channel, string message)
        {
            throw new NotImplementedException();
        }
        
        public Task<bool> Send(ulong channel_id, string message, Stream resource, string filename)
        {
            throw new NotImplementedException();
        }
        
        public Task<bool> Send(ulong channel_id, Stream resource, string filename)
        {
            throw new NotImplementedException();
        }
        
        public async Task<bool> Connect()
        {
            await Task.Delay(1000); // Simulate logging in.
            await OnReady.Invoke();
            throw new NotImplementedException();
        }
        
        public Task<bool> Disconnect()
        {
            throw new NotImplementedException();
        }
        
        public Task<bool> SetAvatar(Stream file_stream)
        {
            throw new NotImplementedException();
        }
        
        public Task<bool> SetUsername(string name)
        {
            throw new NotImplementedException();
        }
        
        public Task<bool> SetNickname(ulong guild, string name)
        {
            throw new NotImplementedException();
        }
        
        public Task<bool> SetStatus(string status)
        {
            throw new NotImplementedException();
        }
        
        public Task PingLoop(CancellationToken token, int timeout, int tries)
        {
            throw new NotImplementedException();
        }
    }
}
