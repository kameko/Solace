
namespace Solace.Modules.Discord.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Services.Providers;
    using Solace.Core;
    
    public class DiscordConfig
    {
        public string ConnectionToken { get; set; }
        public DateTime DeleteMessagesFromDatabaseOlderThan { get; set; }
        public int PingTimeout { get; set; }
        public int PingTries { get; set; }
        public bool DebugLog { get; set; }
        public Log.LogLevel LogLevel { get; set; }
        
        public DiscordConfig()
        {
            ConnectionToken = string.Empty;
            DeleteMessagesFromDatabaseOlderThan = new DateTime(2009, 1, 1);
            PingTimeout     = 5000;
            PingTries       = 3;
            DebugLog        = true;
            LogLevel        = Log.LogLevel.Debug;
        }
        
        public ConfigurationElement ToConfig()
        {
            var cfg = new ConfigurationElement();
            cfg.Configuration.TryAdd(nameof(ConnectionToken), ConnectionToken);
            cfg.Configuration.TryAdd(nameof(DeleteMessagesFromDatabaseOlderThan), DeleteMessagesFromDatabaseOlderThan.ToString());
            cfg.Configuration.TryAdd(nameof(PingTimeout), PingTimeout.ToString());
            cfg.Configuration.TryAdd(nameof(PingTries), PingTries.ToString());
            cfg.Configuration.TryAdd(nameof(LogLevel), LogLevel.ToString());
            return cfg;
        }
        
        public static DiscordConfig FromConfig(ConfigurationToken config)
        {
            var success = config.Configuration.TryGetValue("Discord", out var elm);
            
            if (success)
            {
                var cfg = new DiscordConfig();
                
                success = elm!.Configuration.TryGetValue(nameof(ConnectionToken), out var connection_token);
                if (success)
                {
                    cfg.ConnectionToken = connection_token!;
                }
                
                success = elm.Configuration.TryGetValue(nameof(DeleteMessagesFromDatabaseOlderThan), out var delmsgsstr);
                if (success)
                {
                    success = DateTime.TryParse(delmsgsstr, out var delmsgsdt);
                    if (success)
                    {
                        cfg.DeleteMessagesFromDatabaseOlderThan = delmsgsdt;
                    }
                }
                
                success = elm.Configuration.TryGetValue(nameof(PingTimeout), out var pingti);
                if (success)
                {
                    success = int.TryParse(pingti, out var pingt);
                    if (success)
                    {
                        cfg.PingTimeout = pingt;
                    }
                }
                
                success = elm.Configuration.TryGetValue(nameof(PingTries), out var pingtri);
                if (success)
                {
                    success = int.TryParse(pingti, out var pingtr);
                    if (success)
                    {
                        cfg.PingTries = pingtr;
                    }
                }
                
                success = elm.Configuration.TryGetValue(nameof(DebugLog), out var dbglstr);
                if (success)
                {
                    success = bool.TryParse(dbglstr, out var dbgl);
                    if (success)
                    {
                        cfg.DebugLog = dbgl;
                    }
                }
                
                success = elm.Configuration.TryGetValue(nameof(LogLevel), out var loglstr);
                if (success)
                {
                    success = Enum.TryParse(typeof(Log.LogLevel), loglstr, true, out var loglo);
                    if (success)
                    {
                        var logl = (Log.LogLevel)loglo!;
                        cfg.LogLevel = logl;
                    }
                }
                
                return cfg;
            }
            
            throw new InvalidOperationException("No Discord config element found");
        }
    }
}
