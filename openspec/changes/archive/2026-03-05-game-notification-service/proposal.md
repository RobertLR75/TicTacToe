## Why

The platform currently has no dedicated service for delivering notifications (e.g., game invitations, turn alerts, match results). As more real-time and asynchronous features are planned, a standalone notification API is needed now so that other services can integrate against a stable contract from the start.

## What Changes

- Add a new **GameNotificationService** ASP.NET Core project using FastEndpoints as a blank/stub API.
- Register the service in the Aspire AppHost with appropriate resource references.
- Wire up ServiceDefaults (OpenTelemetry, health checks, service discovery).
- Expose placeholder endpoints that return empty/stub responses, ready for future implementation.

## Capabilities

### New Capabilities
- `notification-api`: Blank FastEndpoints API project scaffolded with health checks, OpenTelemetry, and Aspire integration. Provides stub endpoints for future notification delivery.

### Modified Capabilities
<!-- No existing capabilities are changing at the spec level. -->

## Impact

- **New project**: `src/Backend/GameNotificationService/` added to the solution.
- **Aspire AppHost**: New project reference and resource registration in `AppHost.cs`.
- **Solution file**: Updated to include the new project.
- **Dependencies**: No new external NuGet packages beyond what ServiceDefaults already provides.
- **APIs**: No breaking changes; new endpoints are additive stubs.
- **Rollback plan**: Remove the project reference from the AppHost and solution file; delete the `GameNotificationService` directory.
- **Affected teams**: Backend team (new project ownership), frontend team (awareness of future integration point).
- **Performance impact**: Negligible -- the service is a blank API with no processing logic. Aspire will spin up one additional container/process during local development.
