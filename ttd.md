# Things to do

## Features
- Asynchronous plugins
	- If an asynchonrous plugin adds steps, should it add them to a separate pipeline?
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
