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
The PipelinesCE system consists of four separate levels.
### PipelinesCE
PipelinesCE is the heart of the system. It manages plugins and provides an interface for user facing clients like the command line application.
### PluginTools
Plugin tools are types utilizes by plugins. These types are also referenced in PipelinesCE, therefore, to avoid circular dependencies, they cannot be part of PipelinesCE.
### Plugins
- Dependency 
- Runtime data (e.g plugin options, remaining steps) 
  - Provide through api method parameters. Messing with services at runtime or using complex factories causes inflexibility and convolution.
    Good write up on handling runtime data: https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=99.
### CommandLineApp
The command line application provides a simple way for end users to utilize PipelinesCE.
#### Dependency Injection
The command line application creates two containers, one for its own services and another for PipelinesCE. This accomplishes 
two things:
- Avoids bleeding of CLA services into PipelinesCE plugins. PipelinesCE is a plugin based tool. Having CLA services be accessible
to plugins can be troublesome. For example, if users attempt to use such plugins in a system with a different user facing layer.
- Avoids use of service locator in RunCommand. ILoggerFactory can only be configured for PipelinesCE after command line switches
have been parsed. This means that if only one container is used, IServiceProvider must be injected into RunCommand PipelinesCE 
can be instantiated after ILoggerFactory is configured.
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
### Shared Systems
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
