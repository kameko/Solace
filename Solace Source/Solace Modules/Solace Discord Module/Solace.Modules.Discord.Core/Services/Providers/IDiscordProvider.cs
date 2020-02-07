
namespace Solace.Modules.Discord.Core.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IDiscordProvider : IProvider
    {
        event Func<DiscordMessage, Task> OnReceive;
    }
}
