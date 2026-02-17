# BuildModelAsync Pattern

Use a private `BuildModelAsync()` method whenever a page has both `OnGet` and `OnPost` that can re-render the same view (e.g. on validation failure).

```csharp
public async Task<IActionResult> OnGet(string? returnUrl)
{
    await BuildModelAsync(returnUrl);
    return Page();
}

public async Task<IActionResult> OnPost()
{
    // ... handle submission ...

    // On failure, re-render with same view state:
    await BuildModelAsync(Input.ReturnUrl);
    return Page();
}

private async Task BuildModelAsync(string? returnUrl)
{
    Input = new InputModel { ReturnUrl = returnUrl };
    View = new ViewModel { /* ... populate from services ... */ };
}
```

- Always `private async Task` — even if no async work is currently needed
- Sets both `Input` and `View` from scratch — do not partially update them
- Centralizes all view initialization; `OnGet`/`OnPost` should not set `View` directly