# Client Grant Types

Two patterns — pick based on whether a human user is involved:

**User-facing clients** (browser, mobile app):
```csharp
AllowedGrantTypes = GrantTypes.Code,
RequirePkce = true,
AllowOfflineAccess = true,  // always include for refresh tokens
```

**Machine-to-machine clients** (service, daemon):
```csharp
AllowedGrantTypes = GrantTypes.ClientCredentials,
// No AllowOfflineAccess — refresh tokens don't apply
```

- Never use `ClientCredentials` for a client with a real user
- `RequirePkce = true` is mandatory for all `Code` flow clients