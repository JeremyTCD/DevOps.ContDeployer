# PipelinesCE
PipelinesCE (continuous everything) is a tool for automating builds, integration and deployment. It is designed to minimize cognitive overhead for C# developers.

## Goals
### Low cognitive overhead   
PipelinesCE keeps cognitive overhead low by using technologies and patterns that C# developers are already familiar with.
- All interactions with PipelinesCE are done through a C# API rather than a DSL
- Plugins are Nuget packages
- Scripting is done in C#
### Modularity and reusability
PipelinesCE facilitates reusability through a modular architecture.
- Plugins are easy to write and use
- Each plugin can define its own IOC container
### Responsive pipelines
PiplinesCE allows manipulation of pipeline steps while a pipeline is executing. This dynamism facilitates organizing of logic that 
responds to different situations.
- Adapters can be used to adapt plugins to specific situations
### Cross platform
PipelinesCE is built using .net core for cross platform support.
### Distributed
PipelinesCE keeps all relevant data within the project or solution being worked on. This means that there is no need to configure and manager a central server with
plugins.

## System Architecture
The PipelinesCE system consists of three levels and two shared libraries.
### Levels
From top (closest to end users) to bottom:
#### PipelinesCE.CommandLineApp
Provides a user facing interface. Converts user input into instances of options Types from PipelinesCE.Tools. Uses ProjectRunner to run 
config project with serialized options instances as arguments.  
Config project must reference same PipelinesCE.Tools version so that it can deserialize options in a predictable manner.
#### PipelinesCE.Runner
Referenced by config projects, PipelinesCE.Runner contains a main method that ProjectRunner locates and calls. Loads
config project assemblies that reference PipelinesCE.Tools. Intantiates instances of relevant types, sets up DI and 
finally runs Pipeline defined in config project.
#### PipelinesCE.Plugin
Runs within a Pipeline. 
### Shared Libraries
#### PipelinesCE.Tools
Defines types shared by all levels.
#### ProjectRunner
Restores, builds and publishes a project. Locates main method within generated assemblies and calls it.
### Misc
#### Dependency Injection
The command line application creates a single StructureMap IContainer. This IOC container contains services for PipelinesCE and CLA. StructureMap is used for its
tenancy features. Each plugin has its own child Container. This ensures that services registered for one plugin do not intefere with services registered for another.
#### Commands
##### Name
Should be a lowercase verb, for example "run". 
##### Full Name
Name but with case intact, for example "Run".
##### Description
Should be of the form <what it does><elaboration/notes>.
#### Options
##### Long Name
An option that requires values should have a noun for its long name. For example, "pipeline" or "project". 
An option that does not take a value can be a verb or a noun. For example, "dryrun" or "version".
##### Short Name
An option's short name should be the first alphabet of its long name. If multiple options have the same first alphabet for their long names, the
first alphabet of each option's second syllable should be added to their short names. For example, "pj" for "project" and "pl" for "pipeline".
##### Description
If options is a noun, its description should be in the following format: <what it is><elaboration/notes><default value>. If options is a verb, its description
should be in the following format: <what it does><elaboration/notes>.
##### Defaults
PipelinesCE allows default options to be specified in PipelineFactorys. Therefore, every option must have at least two states: specified or unspecified (overridable by defaults). These states are facilitated
by creating a nullable field in PipelineOptions for every property. If a field is null, the option is unspecified and can be overridden by defaults. Note that since flag CLA options can only be on or off,
every flag must have a corresponding "<flag>off" CLA option. 
#### Logging Strategy
##### Info Level Log
The goal of messages at this log level is to provide users with a quick way to verify that PipelinesCE is behaving in the expected manner. It is thus a rough 
overview of what PipelinesCE is doing with an emphasis on operations affected by user input. For example, it should begin by notifying the user that project 
"example project" has been built since the user specifies which project to use. It should not inform the user that PipelinesCE.Run has been called since the 
call occurs regardless of user input and is unaffected by user input. Messages at the info log level are essentially part a console application's user 
interface. Therefore, they should be written to be as user-friendly as possible:
- Simple and consistent verbs and nouns
- Every log message that marks the beggining of an operation should have a corresponding end message
- Messages should explain ambiguous terms
##### Debug Level Log
The goal of messages at this log level is to aid in debugging. A common scenario is the throwing of an exception by some lower level library. When this occurs,
a stack trace and a message gets printed. Those hints alone can be insufficient for determining the problem. For example, an IndexOutOfRangeException can be
thrown in a function. In such a scenario we would have no clue what the index was, or the contents of the collection. To fill in these gaps, debug level log messages
should be used to provide context. This means it is insufficient to say "Running pipeline", rather what would be useful would look more like "Running pipeline
with options <insert options here>". Therefore, log debug level messages wherever additional context can be provided. Often this means logging arguments
immediately after entering functions. Debug logs are not a magic bullet, sometimes it will be necessary to recreate conditions and run with the debugger attached.
##### Testing Logging
The goal of testing logging is ensuring that log messages are written at the expected log levels with the expected interpolations. 
## Plugins
### XUnit 
#### What it should do
- If assemblies are included in options, run them
- If no project is specified, run all that reference the xunit assembly
- If assemblies are excluded in options, run all that reference the xunit assembly other than the excluded ones
- Option to kill pipeline if test fail
- Test results should be output to xml/json