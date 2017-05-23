# ReadMe
## Architecture
### PipelinesCE
### Plugins
- Dependency 
- Runtime data (e.g plugin options, remaining steps) 
  - Provide through api method parameters. Messing with services at runtime or using complex factories causes inflexibility and convolution.
    Good write up on handling runtime data: https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=99.