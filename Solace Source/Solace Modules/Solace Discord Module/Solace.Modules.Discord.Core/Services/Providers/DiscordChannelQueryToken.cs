
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    // TODO: have a Provider feed this with Discord messages
    // that the consumer can query
    
    public class DiscordChannelQueryToken : IDisposable, IEnumerable<SolaceDiscordMessage>
    {
        private IDiscordProvider Provider { get; set; }
        
        public DiscordChannelQueryToken(IDiscordProvider provider)
        {
            Provider = provider;
        }
        
        public IEnumerator<SolaceDiscordMessage> GetEnumerator()
        {
            throw new NotImplementedException();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
