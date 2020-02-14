
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public class DiscordChannelQuery : IAsyncEnumerable<SolaceDiscordMessage>
    {
        public static readonly int DefaultRequestDelay = 1_000;
        
        private IDiscordProvider Provider { get; set; }
        private ulong ChannelId { get; set; }
        private ulong InitialMessageId { get; set; }
        private List<SolaceDiscordMessage>? CurrentBatch { get; set; }
        private int RequestDelay { get; set; }
        
        public DiscordChannelQuery(IDiscordProvider provider, ulong channel_id, ulong message_id)
        {
            Provider         = provider;
            ChannelId        = channel_id;
            InitialMessageId = message_id;
            CurrentBatch     = null;
            RequestDelay     = DefaultRequestDelay;
        }
        
        public async Task<bool> Setup()
        {
            CurrentBatch = new List<SolaceDiscordMessage>();
            
            var message = await Provider.GetMessage(ChannelId, InitialMessageId);
            if (message is null)
            {
                return false;
            }
            
            CurrentBatch.Add(message);
            return true;
        }
        
        public void SetRequestDelay(int milliseconds)
        {
            RequestDelay = milliseconds;
        }
        
        private async Task<bool> SetNewBatch(ulong id)
        {
            var msgs = await Provider.QueryBefore(ChannelId, id, Provider.MaxQueryLimit);
            if (!(msgs is null))
            {
                CurrentBatch = new List<SolaceDiscordMessage>(msgs.OrderByDescending(x => x.Created));
                return true;
            }
            return false;
        }
        
        public async IAsyncEnumerator<SolaceDiscordMessage> GetAsyncEnumerator(CancellationToken token)
        {
            SolaceDiscordMessage current = CurrentBatch.First();
            yield return current;
            
            var success = await SetNewBatch(current.MessageId);
            if (!success)
            {
                yield break;
            }
            
            for (int i = CurrentBatch!.Count - 1; i >= 0; i--)
            {
                current = CurrentBatch.ElementAt(i);
                if (i == 0)
                {
                    success = await SetNewBatch(current.MessageId);
                    if (!success)
                    {
                        yield break;
                    }
                    
                    i = CurrentBatch.Count; // Intentionally not Count - 1, for loop decrements for us.
                    await Task.Delay(RequestDelay);
                }
                
                yield return current;
            }
        }
    }
}