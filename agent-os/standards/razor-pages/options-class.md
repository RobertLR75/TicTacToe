# Feature Options Class

When a feature has configurable behavior (timeouts, flags, messages), put constants in a co-located static `*Options.cs` file.

```csharp
// Pages/Account/Login/LoginOptions.cs
namespace IdentityService.Pages.Account.Login;

public static class LoginOptions
{
    public static readonly bool AllowLocalLogin = true;
    public static readonly bool AllowRememberLogin = true;
    public static readonly TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
    public static readonly string InvalidCredentialsErrorMessage = "Invalid username or password";
}
```

- Use `static readonly` — these are not config-driven; changes require redeployment
- Reference from the PageModel directly: `LoginOptions.AllowRememberLogin`
- Do not scatter magic values (strings, timeframes) directly in `Index.cshtml.cs`