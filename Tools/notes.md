
### General Plans/TODOs:
 - Version the entire system independently, the initial release will be 1.0.0.0. When a module's minor version is updated, it will tick the system build version (third place) by one. When a plugin's major version is updated, it will tick the system minor version by one. Not sure what to do with the last revision digit yet.
 - Create an extension to the module system to allow for raw C# code to be compiled into a DLL and loaded at runtime, to skip the manual build step. Also enable it to work with Git remote repositories.
 - Consider a permissions system for modules. Obviously wouldn't be any kind of actual security or sandboxing, but it might be nice for modules to document what OS APIs they access, and potentially detect/report if they use any they shouldn't (so the module developer can properly document it).
 - Need a user system with permissions for local and remote connections to the bot.
 - Have the system batch-save how long it's been running to the database. Saving every second it's running is wasteful, so we'll wait for something that wants to save before saving the current timestamp. If nothing wants to save after a while, then we'll manually save the running time. Should be configurable, but by default, maybe every 5 minutes?
 - Have the Server fork itself and use Akka Remoting to communicate with the fork. The fork (slave) will handle modules and the core (master) will supervise it and handle clients/commands. By default there is only one module slave, but moduels can be forked into new slave processes during runtime. This way even a critical fault in a module cannot bring the system down. Also allow the system to run single-instance and load modules in the master directly, but warn that it's dangerous (make exceptions for warnings for ModuleKind.System modules if we ever want to load some critical core module into the master even if we are using slave processes).
 - We will have three Module Servers (MODSERV) by default (Although really it'll be more than three MODSERVs, because every scripting language gets it's own MODSERV, and I plan to at least ship JavaScript at launch), one for trusted system modules and one for noncritical modules. Trusted modules are things like networking protocols, noncritical are things like Discord. The third MODSERV will be another trusted one, but all it will do is monitor system performance inside and out from the safety of it's own process. We need this for clients so the client can read system health information.
 - Conditionally load modules. Primarily for selecting specific modules for specific platforms, for example `SomeModule.Windows.x64` on Windows and `SomeModule.Linux.x64` on Linux.
 - Give Solace.Server redundancy and failover capabilities by offering to host instances that are idle and waiting for the central server to stop responding and take over. Same for module servers.
 - Consider programmatic docker deployment of server instances with https://github.com/microsoft/docker.dotnet 
 - General-purpose analytics engine module that can be utilized by other modules and fed data to it. Both to track performance and runtime behavior and to formulate popularity or lack of popularity with any given data set (eg, what words were most used in a given discord channel, etc.)
 - Need a nice way to set up and describe actors who are willing to accept specific messages. Like a Service actor will accept requests for a specific resource whereas a Client actor will accept messages to send to a client.
 - Some plugins will need some kind of common data types for message passing, look into how assembly loading handles this.
 - Lazy start module servers. Don't start a module server if it has no module configs, and kill it if it has no modules to run.
 - User system is a module that communicates with the database module.
 - Perform inter-module communication by sending each module a reference to the module's supervisor actor (IActorReference) on startup and every time a new module loads. The module can then save those references and do whatever it likes with them.

### Database TODOs:
 - Use Postgres for now. Planned to write own custom database solution in the relatively-far future. Use EF core for both.
 - https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/
 - Use PostgreSQL for the database and just use EF Core for querying between actors. Use some kind of embedded database that has an EF provider as a backup if no Postgres can be connected to (maybe SQLite, but also check if LiteDB is getting one since it has SQL support now). We'll use EF Core as our database-agnostic querying system so that database providers can be modules.

### Discord TODOs:
 - Scriptable response system: Users should be able to use a scripting language to create intelligent responses.
 - System will automatically download all attachments when received. It will do this by categorizing attachments into a folder for that day, in a folder for that channel, in a folder for that guild. Each attachment will have an accompanying JSON file of the same name with details on what day it was, who uploaded it, what was their discord roles, what Solace permissions do they have, what was the current name of the guild, what the attaching message said, etc..
 - All downloaded attachments will be available to response scripts. A script can say to upload a file if they know the date/channel/guild/filename. Alternatively this system will simply allow a user to upload a file and get a GUID back for it, and they can just use that GUID. A response script that be used with this guid to create a memorable tag for uploading the GUID.
 - Periodically, the system will check duplicate files, delete the duplicate while keeping the JSON file, and update the JSON file to say what it is a duplicate of so the user can search there. The above system of accessing a file through it's date/channel/guild/filename will still function, it will just be redirected.
 - Anti-spam system to detect and prevent the bot from spamming the same message in a channel. Useful for haywire scripts or a lot of unhandled exceptions. Don't just detect exact messages, also detect messages that start or end the same but have some variety. If antispam activates, it goes on an auto-cooldown. When it activates, it sends a message to the last channel it responded in saying that anti-spam activated and that it can be disabled by saying ".\antispam disable" or something. Make sure to brand it SpamShield®.
 - Seperate the service (Discord .NET) with the functionality (commands and utilities) and make it them interchangable through a common message passing scheme. So the names will be `Solace.Module.Discord` for the functionality and `Solace.Module.Discord.Provider.DiscordNET` for the Discord .NET provider. When the provider module is loaded, it will broadcast a message to the entire system that it is a Discord provider with it's actor ref attached to the message. The functionality core will look out for that message, then save that reference and use that for I/O. It will keep listening out for new providers so that providers can be swapped out at runtime.
 - Command ideas:
   - Make a fake ".\shutdown" command, have the real command be something like ".\initiate routine system-shutdown now". Fake command will have the bot go offline and them come back online. It can also be configured to say something while pretending to shut down and then when coming back online, either through a seperate command (sent through DMs probably) or from a system config.
   - Make a tag system, `.\tag create tagname "my tag contents"`, `.\tag echo tagname`, `.\tag delete tagname`
   - Do the same with uploads, `.\file create tagname` (with file attached), `.\file upload tagname`
   - Quoting feature

### Language TODOs:
 - Create a custom scripting language for scripting the system. SolaceScript (.sol)? Big requirement is it must be able to be preemptively multitasked by pausing between it's execution, something other embeddable languages don't offer.
 - Also use Lua (MoonSharp) and JavaScript/TypeScript (Jint) for users more familiar with them. Keep them in their own process so they can be killed.
 - Jint has a memory limit and timeout feature. We can use this in conjunction with not allowing long-running loops and creating a framework to handle iteration. Example:
 ```cs
    var result = new Engine(options =>
    {
        options.LimitRecursion(1);
        options.Strict();
        options.DiscardGlobal();
        options.TimeoutInterval(TimeSpan.FromSeconds(0.5));
        options.MaxStatements(1);
        options.LimitMemory(500);
    })
    .Execute(javascript)
    .GetCompletionValue() 
    .ToObject();
 ```
 - It does not seem MoonSharp has the same abilities. Consider a Lua to JS converter such as https://github.com/mherkender/lua.js
 - MoonSharp is still fine either way because even if we can trust Jint, it's still going in it's own Language Server process seperate from other modules (which should also be able to be put into their own docker). However, we should split all languages into their own server process, so faulty MoonSharp scripts won't tred on trustworthy Jint scripts.
 - Lua is low-priority, don't bother until you actually want to.
 - Scripts should be trusted or untrusted, trusted scripts aren't subject to timeouts and other hard limits, so they can do more. Trusted scripts are scripts that have been verified and built into the system. Untrusted is user scripts, which are allowed to call trusted scripts to accomplish more.

### Frontend TODOs:
 - The default frontend, Terminal, is part of the Core folder (otherwise you couldn't communicate with the server), but all other frontends will be their out project outside of the Core folder.
 - Three frontends planned. Terminal is a basic bare-bones command frontend. After that I'd like to do another console based one but much fancier, a text-based windowing system (Solace.Pathos). The third one will be a full-blown ASP .NET/Vue .js browser frontend (Solace.Catharsis). All of these are designed with cross-platform in mind.
 - All frontends use TCP and can work across computers, there is no local-only frontend.

### Personality layers:
 - Make server-chan not respond to any message with her trigger in it that ends in a dot (.) Make it configurable too.
 - Make server-chan more autonomous and context-sensitive. Give her a mode to toggle between classic mode (the simple markov chain) and smart mode, such as `.\personality server mode classic`. Or maybe just `.\personality classic`, not sure how I want to do the "personality infrastructure" yet.
