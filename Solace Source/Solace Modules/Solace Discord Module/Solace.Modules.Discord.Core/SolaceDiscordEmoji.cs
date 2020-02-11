
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class SolaceDiscordEmoji
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public Uri Url { get; set; }
        
        public SolaceDiscordEmoji()
        {
            Name = string.Empty;
            Url  = new Uri("https://none.none/");
        }
        
        public void TrySetUrl(string url)
        {
            var success = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri);
            if (success)
            {
                Url = uri!;
            }
        }
    }
}
