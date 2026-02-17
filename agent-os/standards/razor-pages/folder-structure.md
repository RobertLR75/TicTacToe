# Razor Pages Folder Structure

Each feature lives in its own folder under `Pages/`. Folder name = feature name.

**Always present:**
```
Pages/Account/Login/
├── Index.cshtml          # Razor template
├── Index.cshtml.cs       # PageModel
└── ViewModel.cs          # Display data
```

**Add when needed:**
```
├── InputModel.cs         # Form submission data (only for pages with forms)
└── LoginOptions.cs       # Config constants (only when feature has configurable behavior)
```

**Namespace must match folder path:**
```csharp
namespace IdentityService.Pages.Account.Login;
```

- One feature per folder — do not share folders between unrelated features
- Sub-features (e.g. Account/Login, Account/Logout) each get their own folder