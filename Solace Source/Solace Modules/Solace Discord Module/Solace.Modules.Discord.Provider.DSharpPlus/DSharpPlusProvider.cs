
namespace Solace.Modules.Discord.Provider.DSharpPlus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services.Providers;
    using global::DSharpPlus;
    using global::DSharpPlus.EventArgs;
    
    public class DSharpPlusProvider : BaseDiscordProvider
    {
        private DiscordClient Client { get; set; }
        
        public DSharpPlusProvider() : base()
        {
            Client = null!;
        }
        
        public override Task Setup(string token)
        {
            return Task.Run(() =>
            {
                Client = new DiscordClient(new DiscordConfiguration
                {
                    Token     = token,
                    TokenType = TokenType.Bot,
                });
                
                Client.MessageCreated += OnMessageCreated;
            });
        }
        
        public override async Task Connect()
        {
            await Client.ConnectAsync();
        }
        
        private Task OnMessageCreated(MessageCreateEventArgs message)
        {
            throw new NotImplementedException();
        }
    }
}
