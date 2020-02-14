
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class SolaceDiscordEmoji
    {
        public string Name { get; set; }
        public string DiscordName { get; set; }
        public ulong Id { get; set; }
        public bool RequiresColons { get; set; }
        public Uri Url { get; set; }
        
        public SolaceDiscordEmoji()
        {
            Name        = string.Empty;
            DiscordName = string.Empty;
            Url         = new Uri("https://none.none/");
        }
        
        public void TrySetUrl(string url)
        {
            var success = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri);
            if (success)
            {
                Url = uri!;
            }
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is SolaceDiscordEmoji other)
            {
                return Name        == other.Name
                    && DiscordName == other.DiscordName
                    && Id          == other.Id;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return new
            {
                Name,
                DiscordName,
                Id
            }
            .GetHashCode();
        }
    }
}
