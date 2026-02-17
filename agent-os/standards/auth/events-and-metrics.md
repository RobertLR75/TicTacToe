# Auth Events and Metrics

On login success and failure, always raise an `IEventService` event. Also call `Telemetry.Metrics` when available.

**Login success:**
```csharp
await _events.RaiseAsync(new UserLoginSuccessEvent(
    user.Username, user.SubjectId, user.Username,
    clientId: context?.Client.ClientId));
Telemetry.Metrics.UserLogin(context?.Client.ClientId, IdentityServerConstants.LocalIdentityProvider);
```

**Login failure:**
```csharp
await _events.RaiseAsync(new UserLoginFailureEvent(
    Input.Username, error,
    clientId: context?.Client.ClientId));
Telemetry.Metrics.UserLoginFailure(context?.Client.ClientId, IdentityServerConstants.LocalIdentityProvider, error);
```

- `_events.RaiseAsync()` is required — feeds the `IEventSink` for audit logging
- `Telemetry.Metrics` is optional but should be included when the metric exists
- Always pass `clientId` from context so events are correlated to the requesting client