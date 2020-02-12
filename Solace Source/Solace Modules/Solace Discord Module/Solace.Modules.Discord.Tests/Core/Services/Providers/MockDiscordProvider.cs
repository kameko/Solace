
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
        public bool Connected { get; protected set; }
        public int MaxQueryLimit { get; protected set; }
        public event Func<Task> OnReady;
        public event Func<SolaceDiscordMessage, Task> OnReceiveMessage;
        
        public MockDiscordProvider()
        {
            MaxQueryLimit    = 100;
            OnReady          = delegate { return Task.CompletedTask; };
            OnReceiveMessage = delegate { return Task.CompletedTask; };
        }
        
        public async Task RaiseOnReceiveMessage(SolaceDiscordMessage message)
        {
            await OnReceiveMessage.Invoke(message);
        }
        
        public Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id, ulong starting_message_id)
        {
            throw new NotImplementedException();
        }
        
        public Task<DiscordChannelQueryToken?> QueryChannel(ulong channel_id)
        {
            throw new NotImplementedException();
        }
        
        public Task<SolaceDiscordMessage?> GetMessage(ulong channel_id, ulong message_id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
