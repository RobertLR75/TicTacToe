# ViewModel and InputModel

**ViewModel** — display/read data populated by the page model:
```csharp
public ViewModel View { get; set; } = default!;
```
- May contain computed properties (e.g. `IsExternalLoginOnly`, `VisibleExternalProviders`)
- Set in `BuildModelAsync()` or `OnGet` — never bound from form submission

**InputModel** — form submission data:
```csharp
[BindProperty]
public InputModel Input { get; set; } = default!;
```
- Only form fields that the user submits
- Use `[Required]` for mandatory fields
- Include `Button` (string) to identify which submit button was clicked

```csharp
public class InputModel
{
    [Required] public string? Username { get; set; }
    [Required] public string? Password { get; set; }
    public string? ReturnUrl { get; set; }
    public string? Button { get; set; }  // "login", "cancel", etc.
}
```