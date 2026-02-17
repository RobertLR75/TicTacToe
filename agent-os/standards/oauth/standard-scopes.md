# Standard OIDC Scope References

Use `nameof()` + `.ToLower()` for standard OIDC scopes — never string literals.

```csharp
// Correct
AllowedScopes =
{
    nameof(IdentityServerConstants.StandardScopes.OpenId).ToLower(),   // "openid"
    nameof(IdentityServerConstants.StandardScopes.Profile).ToLower(),  // "profile"
    nameof(ApiScopeNames.ArrangementRead)
}

// Wrong — magic strings
AllowedScopes = { "openid", "profile", "ArrangementRead" }
```

- `.ToLower()` is required because `StandardScopes` enum values are PascalCase (`OpenId`), but the OIDC spec expects lowercase (`openid`)
- Custom API scopes from `ApiScopeNames` do NOT need `.ToLower()` — the enum value is already the exact scope name
