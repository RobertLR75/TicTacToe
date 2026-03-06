## Why

The current frontend does not verify whether a user session exists before rendering the home experience, which allows anonymous access paths and inconsistent user identity state. We need a minimal session gate now so every visit has a valid user identity in session before entering gameplay flows.

## What Changes

- Add a frontend session guard that checks for an authenticated session user (`UserId` GUID and `Name`) before allowing access to the home page.
- Add a simple login page where users provide a username when no session exists.
- On successful username submit, create a new session user object (`UserId`, `Name`) and store it in session.
- Redirect unauthenticated users from the home page to the login page, then redirect back to home after session creation.
- Add validation for empty/whitespace usernames and show a basic error message without creating session state.
- Add automated coverage for session guard behavior, login submission, and redirect flow.

## Capabilities

### New Capabilities
- `frontend-session-login-gate`: Require a session-backed user identity before accessing the homepage and provide a login flow that creates session identity.

### Modified Capabilities
- None.

## Impact

- Affected code: Frontend routing/entry flow, login UI, session state utilities, and related tests.
- APIs/dependencies: No external API or package changes expected; uses existing server session middleware.
- Systems: Session storage usage increases slightly because each user receives `UserId` and `Name` values.
- Performance impact analysis: Negligible runtime overhead (single session lookup on entry and lightweight form submit); no expected gameplay performance regression.
- Affected teams: Frontend team (implementation), QA team (flow validation), and platform team (session configuration awareness).
- Rollback plan: Revert the session-guard routing and login page wiring to restore current direct homepage access, then redeploy.
