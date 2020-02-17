
namespace Solace.Server
{
    using System;
    using System.Threading.Tasks;
    using Core;
    using Serilog;
    
    class Program
    {
        static async Task Main(string[] args)
        {
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            
            Core.Log.AttachSerilog();
            
            var system = new SystemManager(null);
            await system.Setup();
            system.Start();
            
            Console.ReadLine();
            
            system.Stop();
            
            // Console.ReadLine();
        }
    }
}
