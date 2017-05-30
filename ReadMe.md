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