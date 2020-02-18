
namespace Solace.Server
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    
    // TODO: custom serilog sink for console.
    // custom console interface that has a text input
    // field at the bottom.
    
    class Program
    {
        static async Task Main(string[] args)
        {
            // TODO: parse args for:
            // - (--config, -c) config path (default is null, which is "./solace.conf")
            // - (--logconfig, -lc) log config path (a json file) (default is null, which is "./solace.log.conf")
            // - (--logjson, -lj) raw log config JSON. conflicts with --logconfig
            
            // TODO: create log file config handler. make sure it's smart enough to
            // tell solace.conf from solace.log.conf in case user mixes them up in
            // the command line arguments and breaks everything.
            
            Logging.SetupLogger();
            
            Core.Log.Info("Starting Solice");
            
            var system = new SystemManager(null);
            await system.Setup();
            system.Start();
            
            Console.ReadLine();
            
            system.Stop();
            
            // Console.ReadLine();
        }
    }
}
