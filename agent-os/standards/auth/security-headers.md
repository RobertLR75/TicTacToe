# Security Headers

Apply `[SecurityHeaders]` to every Razor Page — no exceptions.

```csharp
[SecurityHeaders]
[AllowAnonymous]       // or [Authorize]
public class Index : PageModel { ... }
```

The attribute adds these headers to all `PageResult` responses:
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `Content-Security-Policy: default-src 'self'; object-src 'none'; frame-ancestors 'none'; ...`
- `Referrer-Policy: no-referrer`

- Do not modify `SecurityHeadersAttribute` to loosen the CSP
- `[SecurityHeaders]` must come before `[AllowAnonymous]` / `[Authorize]`