# OIDC Cancel / Deny Pattern

Any interactive page with a cancel button must signal denial back to IdentityServer via `DenyAuthorizationAsync`.

```csharp
// User clicked cancel (Input.Button != "login" / "yes" / etc.)
var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
if (context != null)
{
    ArgumentNullException.ThrowIfNull(Input.ReturnUrl, nameof(Input.ReturnUrl));
    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

    return context.IsNativeClient()
        ? this.LoadingPage(Input.ReturnUrl)   // better UX for native apps
        : Redirect(Input.ReturnUrl ?? "~/");
}
return Redirect("~/");
```

Applies to: Login, Consent, Device, CIBA — any page that can abort an OIDC flow.

- Never redirect to `ReturnUrl` without first calling `DenyAuthorizationAsync` on cancel
- Always check `IsNativeClient()` and return `LoadingPage()` for native clients