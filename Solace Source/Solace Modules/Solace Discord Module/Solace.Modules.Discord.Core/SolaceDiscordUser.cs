
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
        public string Nickname { get; set; }
        public IEnumerable<SolaceDiscordRole> Roles { get; set; }
        public ulong Id { get; set; }
        public bool IsBot { get; set; }
        public Uri AvatarUrl { get; set; }
        public string AvatarHash { get; set; }
        
        public SolaceDiscordUser()
        {
            Username   = string.Empty;
            Nickname   = string.Empty;
            Roles      = new List<SolaceDiscordRole>();
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
            return ObjectExtensions.GetShallowObjectDifferencesAsString(this, other);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is SolaceDiscordUser other)
            {
                return Username      == other.Username
                    && Discriminator == other.Discriminator
                    && Id            == other.Id;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return new
            {
                Username,
                Discriminator,
                Id
            }
            .GetHashCode();
        }
    }
}
