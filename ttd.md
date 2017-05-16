# Things to do
- Figure out how to debug external packages
  - http://stackoverflow.com/questions/42773815/net-core-csproj-global-json-projects-equivalent
  - Publish packages locally with pdb?
  - Some way to import projects into current solution?

## Initial Features
- Clean up solution
  - Plugins should end with name plugin
  - Consistent names for tests
- Code based pipeline definition 
  - This requires additions to ContDeployer
  - Instead of reading from a json file, locate project with pipeline definitions, build it and run it
- Nuget plugin
  - Use MSBuild.exe to pack
    - Does /t:pack also build?
  - Figure out how to use nuget to package and deliver exe
  - Test and get it to work. Try locally or try http://staging.nuget.org/
  - Publish ContDeployer to nuget (rename it?)
- Console app
  - Arguments
- Envionment variables plugin

## Future Features
- Nuget plugin
  - Use https://github.com/NuGet/NuGet.Client/tree/dev/src/NuGet.Core/NuGet.Commands
- MSBuild/Roslyn plugin
  - Currently, MSBuild api is quite messy because it is dependent on tasks and targets that cannot
    be specified easily - https://github.com/Microsoft/msbuild/issues/1493.
  - Roslyn's MSBuildWorkspace is not cross-platform since it is also dependent on shitty tasks and 
    targets.
  - A solid solution would be to write a custom MSBuildWorkspace that uses MSBuild's ProjectRootElement
    to initialize. Thereafter Roslyn can be used and MSBuild can be forgotten for good.
- Xunit plugin
- Async/parrallel plugins
  - If an asynchonrous plugin adds steps, should it add them to a separate pipeline?
- Pause and restart pipeline 
  - E.g when manual testing of staging environment needs to be done- 
## Code hygiene
- Move log messages to resx files
  - Faciliates localization 
  - Centralizes strings
- Make public functions virtual
  - No harm in extensibility
## Tests
- Integration tests for plugins
- Mocks for ILogger instances
- Mock for ITimeService
  - Ensure that code calls correct date time type (DateTimeOffset.UtcNow)
- Unit tests for ContDeployer and Plugin tools types
  - All types with logic should be tested
