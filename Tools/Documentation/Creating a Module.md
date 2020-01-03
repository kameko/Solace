
# Creating a Module

## What is a module?

A Solace plugin is called a module. The reason for naming plugins as modules is because of an AssemblyLoadContext issue I faced, which ended up requiring a Microsoft employee to contact me to help straighten out the usability issue. Before he helped me out, I was prepared to just dump plugins and call all Solace extensions "modules", so it's a bit of a historic name. Also I just like the name "module" more than "plugin".

A module, as a plugin, is a self-contained executable that is loaded into the Solace address space and communicates with the rest of the system (including other modules) through Akka .NET message passing. Modules are normally not loaded into the core Solace instance, but rather they are loaded in alternative Solace instances that are booted as dedicated module servers which communicate back to the original Solace instance.

## Module skeleton

All modules must have one, and only one class inheriting from `Solace.Core.Modules.BaseModule`.

A module must first start off by initializing all of it's metadata fields through it's `void Setup(SystemConfiguration config)` method. This can be done through the module's default constructor, too, but the recommended way is through the Setup method. Do not perform any other action than setting up the module's metadata or read the provided SystemConfiguration argument, as the module is not ready yet. Next, the module's supervisor actor provided through it's `ActorDescription GetSupervisorDescription()` to the module server, which then instances that actor. Again, this actor should do nothing and be initialized in an "unstarted" state.

After this, the module is kept in an "unstarted" state while it reads the module's metadata and ensures the system is prepared for running the module. This is primarily for the benefit of the module's dependencies or other modules that depend on your module. After the system is ready to allow the module to run, it performs several steps:
 - The module's `void OnReady()` method is invoked.
 - The module's supervisor is sent a `(Solace.Core.Modules.ModuleMessages.Started, bool)` ValueTuple `(enum, bool)`. The bool is to signify if the module has any state to load or not.

Now the module is ready. If the module depends on any other modules, and those modules unload, it will automatically be paused while it waits for it's dependencies to come back online.

If the module itself is reloading, the module will receive a `ModuleMessages.PreReload` message, at which point it has a few seconds to send it's parent a `(ModuleMessages.SaveState, string)` ValueTuple, the string containing whatever serialized data you want. The string does not need to be in any specific format, it's for your module's benefit only. Then the module will be killed and unloaded, then reloaded with a `(ModuleMessages.PostReload, bool)` message, which it should listen for and receive while it's still in it's paused state. After this, if PostReload's bool value is true, the module should send a `ModuleMessages.LoadState` enum message to it's parent, and get back `(ModuleMessages.LoadState, string)` which will contained the serialized data the module sent before it's reload. The module should set itself to a ready state after this.

Please see the [Example Module](../../Source/Modules/Solace.Module.Test).

```cs
// TODO: create a module skeleton
```

## Communication

TODO: design a good way to communicate with the system and document it.
