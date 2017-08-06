**Work in progress**. This project is incomplete. At present, this readme isn't fully coherent.
*****
# PipelinesCE
PipelinesCE (continuous everything) is a tool for automating building, integrating and deploying. It is designed to minimize cognitive overhead for C# developers.

## Goals
### Low cognitive overhead   
PipelinesCE keeps cognitive overhead low by using technologies and patterns that C# developers are already familiar with.
- All interactions with PipelinesCE are done through a C# API rather than a DSL
- Plugins are Nuget packages
- Scripting is done in C#
- A single pipeline handles everything from building to deploying
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
### Parrellelism and asynchrony
PipelinesCE allows for concurrent operations.

## System Architecture
### Levels
The PipelinesCE system consists of three levels.
From top (closest to end users) to bottom:
#### PipelinesCE.CommandLineApp
Provides a user facing interface. Converts user input into instances of options Types from PipelinesCE.Core. Uses ProjectHost to run 
PipelinesCE.ConfigHost with serialized options instances as arguments.  
#### PipelinesCE.ConfigHost
Referenced by config projects, PipelinesCE.ConfigHost contains an entry method that ProjectHost locates and calls. Loads
config project assemblies that reference PipelinesCE.Core. Intantiates instances of relevant types, sets up DI and 
finally runs Pipeline defined in config project.
#### PipelinesCE.Plugin
Runs within a Pipeline. 
### Shared Libraries
The PipelinesCE system defines 2 shared libraries:
#### PipelinesCE.Core
Defines types shared by all levels.
#### DotNetCore.ProjectHost
Restores, builds and publishes a project. Locates main method within generated assemblies and calls it.
### Sub Systems
#### Configuration
There are two key options types. Both types provide defaults for all options to make it easy to get started.
##### PipelinesCEOptions
The PipelinesCEOptions type in PipelinesCE.Core holds options used by the entire system. These options include `LogFile`, `Verbosity` and `Pipeline`. It is populated 
by PipelinesCE.CommandLineApp (or any app that utilizes PipelinesCE.ConfigHost) and passed to PipelinesCE.ConfigHost.Load.
PipelinesCE.CommandLineApp populates PipelinesCEOptions from command line arguments. In future, it could easily read from a json
file as well using Configuration builder. 
##### SharedPluginOptions
The SharedPluginOptions type in PipelinesCE.Core holds options shared amongst plugins. These options include `DryRun`, ???. PipelinesCEOptions contains an instance of this type,
allowing SharedPluginOptions to be specified via command line arguments (and a json file in future). This type can also be supplied when instantiating a Pipeline in PipelineFactory.CreatePipeline.
SharedPluginOptions from PipelineCEOptions and Pipeline.SharedPluginOptions are merged before a pipeline runs, with the former taking precedence.
#### Dependency Injection
ServiceCollection is used to define services since it is compatible with all Container libraries.
#### Logging
CommandLineApp adds NLog. By default, console and file targets are added to NLog. Debug target can be added and file target can be removed. All libraries allow for overriding of ILoggingService<>. 

## Style Guide
### PipelinesCE.CommandLineApp
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
#### Logging
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
### Testing
- For xunit, tests in the same class or in classes that share the same collection are not run in parallel. Many of the tests in this solution utilize temporary folders. 
Tests that depend on CurrentDirectory should not run in parallel. https://xunit.github.io/docs/running-tests-in-parallel.html.
- Stub project should be named StubProject.<noun>
- If a test uses a StubProject and its usage is read only (e.g reading build output), there is not need to use a temp dir
- If a test uses a StubProject and modifies files (e.g runs msbuild on the project), the project must first be copied to a temp dir
- temp dirs should be named after the test class they belong to

## Plugins
### Roslyn
### XUnit 
#### What it should do
- If assemblies are included in options, run them
- If no project is specified, run all that reference the xunit assembly
- If assemblies are excluded in options, run all that reference the xunit assembly other than the excluded ones
- Option to kill pipeline if test fail
- Test results should be output to xml/json
### Nuget
### Git
### Github
