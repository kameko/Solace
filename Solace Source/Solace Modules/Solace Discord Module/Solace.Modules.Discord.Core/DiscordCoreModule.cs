
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Solace.Core.Services;
    using Solace.Core.Modules;
    using Services;
    
    public class DiscordCoreModule : BaseModule
    {
        public DiscordCoreModule() : base()
        {
            Info = new ModuleInfo()
            {
                Name      = "Solace.Modules.Discord.Core",
                Company   = "Caesura Software Solutions",
                Version   = new Version(0, 0, 0, 1),
                Copyright = "Caesura Software Solutions 2020",
            };
        }
        
        public override IEnumerable<IService> GetServices()
        {
            return new List<IService>()
            {
                new DiscordService()
            };
        }
    }
}
