
## LEGAL

When contributing to this repository, You agree to relinquish all rights and ownership of any and all resources you contribute to the project and it's owner, be it source code, text, image files, algorithms, or any other item within your legal ownership that is merged to this project's repository. You agree that any resources you are contributing to the project are legally copywritten under your name and you hold ownership of all items included. Should you be publishing an item for a third party, this third party agrees to relinquish all copyright ownership of all merged items to the project as well. If this third party does not agree to relinquish their copyright of all contributed items, your contributions will be reverted and all liability will be held against your person.

You agree that any and all attempts to revoke or restore any and all ownership or copyright of any and all submitted materials is legally null and void and dismissable in court.

You agree under no uncertain conditions that the repository owners holds exclusive rights to prevent you from contributing to the project or communicating with any and all repository staff for any amount of time under any conditions or circumstances. Attempting to circumvent these decisions will be considered harassment.

## CONDUCT

You are never to mention the identity, features, names, texts, recordings, websites, actions, beliefs, or philosophies of any one person or group of people (including yourself) anywhere within the project or issues section that is not directly, immediately related to the functionality and contribution of the project. Any and all attempts to contact or propose a claim on any person within or around the project due to their identity, actions or beliefs will result in you immediately being removed from the project and blocked from any further contributions or interactions. Any further attempts to contribute to the project or contact it's staff will be considered harassment.

Any attempts or suggestions to change, replace or remove this policy will result in you immediately being removed from the project and blocked from any further contributions or interactions. Any further attempts to contribute to the project or contact it's staff will be considered harassment.

Any and all forks of this repository that change, replace or remove this policy may also result in the owner(s) of the fork being blocked from this project, depending on the judgement of the staff.

## Bug Reports

When filing a bug report, please provide the following:
 - Solace version, and name and version of all modules in the system. Link the GitHub repository for any third party modules.
 - Operating system name and version.
 - .NET version.
 - System log and/or stack trace.
 - Actions taken in order to reproduce the issue, and what the expected result was.
 - Any software that is installed or not installed on your machine that could potentially interfere with the functionality of the system.

Please note we do not provide support for the Mono runtime or the .NET Framework runtime, as these are deprecated. Expected runtime for running the system and providing bug reports is at least .NET Core 3.0.

## Support

Project is intended to run on .NET Core 3.0 or greater, do not attempt running it on any other version of .NET. Solace makes heavy use of `AssemblyLoadContext`, and will fail on any other version of .NET. Please ensure all modules you write for Solace also target the same version of the .NET runtime as the Solace instance you're writing the modules for is targeting. Otherwise, any bug reports you submit may be ignored.

Project currently does not have access to a computer running Apple® macOS. Bug reports for the system running on .NET Core 3.0+ on Apple® macOS that cannot be reproduced on other operating systems may be given a lower priority. Please suggest the removal of this notification if you have access to a modern, current generation Apple® branded computer running the latest version of Apple® macOS and intend on becoming a contributor to the project.

This project is intended to run on Microsoft® Windows 10 or Linux running systemd. Due to Solace being a service/daemon, we currently do not intend on supporting any UNIX-like system that does not run systemd. FreeBSD is not yet being considered due to this, but may be considered in the future. Apple® macOS currently cannot be considered for support at this time. No other operating systems are considered for support at this time.

## Coding Convention

Please follow the [Microsoft Visual C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).
