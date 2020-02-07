
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class AttachmentToken
    {
        public string FileName { get; set; }
        public ulong Id { get; set; }
        public int FileSize { get; set; }
        public Uri Url { get; set; }
        public Uri ProxyUrl { get; set; }
        
        public AttachmentToken()
        {
            FileName = string.Empty;
            Url      = new Uri("https://none.none/");
            ProxyUrl = new Uri("https://none.none/");
        }
        
        public void TrySetUrl(string url)
        {
            var success = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri);
            if (success)
            {
                Url = uri!;
            }
        }
        
        public void TrySetProxyUrl(string url)
        {
            var success = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri);
            if (success)
            {
                Url = uri!;
            }
        }
    }
}
