
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class Emoji
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public Uri Url { get; set; }
        
        public Emoji()
        {
            Name = string.Empty;
            Url  = new Uri("https://none.none/");
        }
    }
}
