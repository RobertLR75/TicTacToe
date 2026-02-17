# API Scope Names

All custom API scopes are defined in the `ApiScopeNames` enum in `IdentityServer.Contracts`.

```csharp
// IdentityServer.Contracts/ApiScopeNames.cs
public enum ApiScopeNames
{
    ArrangementRead = 1,
    ArrangementWrite,
    NotificationRead,
    NotificationWrite,
    GameRead,
    GameWrite
}
```

Reference scopes via `nameof()` — never use string literals:

```csharp
new ApiScope(nameof(ApiScopeNames.ArrangementRead), "Arrangement Read Access")
AllowedScopes = { nameof(ApiScopeNames.ArrangementRead) }
```

- OIDC standard scopes (`openid`, `profile`) are exempt — use `IdentityServerConstants.StandardScopes`
- Adding a new scope = add to enum first, then register in `Config.cs`