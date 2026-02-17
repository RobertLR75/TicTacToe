# IdentityServer Event Sink

`IdentityServerEventSink` implements `IEventSink` to log authentication events via `ILogger`.

**Required — log all failure events at `LogWarning`:**
- `UserLoginFailureEvent` — log `Username`, `Message`
- `ClientAuthenticationFailureEvent` — log `ClientId`, `Message`
- `TokenIssuedFailureEvent` — log `ClientId`, `Message`
- `ApiAuthenticationFailureEvent` — log `ApiName`, `Message`
- `TokenIntrospectionFailureEvent` — log `ApiName`, `ApiScopes`, `Message`

**Optional — success events at `LogInformation`:**
- `ClientAuthenticationSuccessEvent` — log `ClientId`
- `TokenIssuedSuccessEvent` — log `ClientId`, `Message`

```csharp
public Task PersistAsync(Event evt)
{
    if (evt is UserLoginFailureEvent e)
        logger.LogWarning("UserLoginFailure: {UserName} - {Message}", e.Username, e.Message);
    // ...
    return Task.CompletedTask;
}
```

- Use structured logging with named properties (`{ClientId}`, not `{0}`)
- Never throw — always return `Task.CompletedTask`