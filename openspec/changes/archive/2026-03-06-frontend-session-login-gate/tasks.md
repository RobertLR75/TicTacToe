## 1. Session Identity Foundation

- [x] 1.1 Define a shared session user contract (`UserId`, `Name`) and session key constant used by homepage and login handlers.
- [x] 1.2 Add helper logic to read and validate session user data (including invalid/missing shape handling).

## 2. Route Guard and Login Flow

- [x] 2.1 Implement homepage guard behavior to redirect to login when session user is absent or invalid.
- [x] 2.2 Add login page GET route that renders a simple username form and redirects authenticated sessions to homepage.
- [x] 2.3 Add login POST route that validates username input, creates server-side GUID `UserId`, stores `{ UserId, Name }` in session, and redirects to homepage.
- [x] 2.4 Return validation feedback for empty/whitespace username without creating session data.

## 3. Verification

- [x] 3.1 Add automated tests for homepage redirect behavior with and without valid session user.
- [x] 3.2 Add automated tests for successful login submission creating session identity and redirecting to homepage.
- [x] 3.3 Add automated tests for invalid login submission and authenticated-user login-route redirect behavior.
