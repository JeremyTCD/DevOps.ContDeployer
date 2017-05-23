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

## Architecture
### Plugins
- Dependency 
- Runtime data (e.g plugin options, remaining steps) 
  - Provide through api method parameters. Messing with services at runtime or using complex factories causes inflexibility and convolution.
    Good write up on handling runtime data: https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=99.