
namespace Solace.Modules.Discord.Provider.DSharpPlus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core.Services;
    using Solace.Core.Modules;
    
    public class DSharpPlusModule : BaseModule
    {
        public DSharpPlusModule() : base()
        {
            Info = new ModuleInfo()
            {
                Name      = "Solace.Modules.Discord.Provider.DSharpPlus",
                Company   = "Caesura Software Solutions",
                Version   = new Version(0, 0, 0, 1),
                Copyright = "Caesura Software Solutions 2020",
            };
            
            Dependencies.Add("Solace.Modules.Discord.Core");
        }
        
        public override IEnumerable<IService> GetServices()
        {
            return new List<IService>()
            {
                new DSharpPlusProvider()
            };
        }
    }
}
