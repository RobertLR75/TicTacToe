## 1. Session Logout Flow

- [x] 1.1 Add a session clear operation to `SessionUserStore` and expose a `/logout` endpoint in `src/FrontEnd/TicTacToeMud/Program.cs` that clears the current session user and redirects to `/login`.
- [x] 1.2 Verify the existing login gate continues redirecting unauthenticated requests to `/login` after logout.

## 2. Shared Navigation UI

- [x] 2.1 Update `src/FrontEnd/TicTacToeMud/Components/Layout/MainLayout.razor` to render a top app bar logout action that is visible across pages using the shared layout.
- [x] 2.2 Ensure the logout control uses the shared MudBlazor layout patterns and triggers the logout endpoint with a full redirect.

## 3. Validation

- [x] 3.1 Add or update frontend tests covering logout visibility, session clearing behavior, and redirect to `/login` after sign-out.
- [x] 3.2 Run the relevant frontend test suite and perform a manual login/logout flow check from the home and game pages.
