
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;
    using Solace.Core;
    
    public class SolaceDiscordUser
    {
        public string Username { get; set; }
        public int Discriminator { get; set; }
        public string FullName => $"{Username}#{Discriminator}";
        public ulong Id { get; set; }
        public bool IsBot { get; set; }
        public Uri AvatarUrl { get; set; }
        public string AvatarHash { get; set; }
        
        public SolaceDiscordUser()
        {
            Username   = string.Empty;
            AvatarUrl  = new Uri("https://none.none/");
            AvatarHash = string.Empty;
        }
        
        public void TrySetUrl(string url)
        {
            var success = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri);
            if (success)
            {
                AvatarUrl = uri!;
            }
        }
        
        public string GetUserDifference(SolaceDiscordUser other)
        {
            return ObjectExtensions.GetObjectDifferencesAsString(this, other);
        }
    }
}
