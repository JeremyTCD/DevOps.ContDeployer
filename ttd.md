# Things to do

## Features
- Figure out how to use nuget to package and deliver exe
  - Test and get it to work. Try locally or try http://staging.nuget.org/
- Nuget plugin
  - https://github.com/NuGet/NuGet.Client/tree/dev/src/NuGet.Core/NuGet.Commands
  - Publish ContDeployer to nuget (rename it?)
  - Move plugins to their own solutions?
  - Create flow for building, testing and publishing nuget plugins
- Console app
  - Arguments
- Envionment variables plugin
- Async/parrallel plugins
  - If an asynchonrous plugin adds steps, should it add them to a separate pipeline?
- Pause and restart pipeline 
  - E.g when manual testing of staging environment needs to be done
- MSBuild plugin
- Xunit plugin
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
