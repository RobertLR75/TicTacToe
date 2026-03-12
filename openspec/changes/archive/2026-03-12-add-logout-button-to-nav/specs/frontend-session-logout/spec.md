## ADDED Requirements

### Requirement: Authenticated pages expose a top navigation logout action
The system SHALL display a logout action in the top navigation bar for pages rendered with the main application layout after a valid session user has been established.

#### Scenario: Logout action is visible from the shared app bar
- **GIVEN** a valid session user with `UserId` and `Name` exists
- **WHEN** the user opens a page rendered with the main application layout
- **THEN** the top navigation bar displays a logout action
- **THEN** the logout action is available without navigating away from the current page first

### Requirement: Logout clears the current session identity
The system SHALL clear the persisted session user when the logout action is invoked.

#### Scenario: Logout removes the current session user
- **GIVEN** a valid session user with `UserId` and `Name` exists
- **WHEN** the user invokes the logout action
- **THEN** the system removes the stored session user from the session

### Requirement: Logout returns the user to the login experience
The system SHALL redirect the browser to the login route after logout so the user leaves the authenticated experience immediately.

#### Scenario: Logout redirects to login
- **GIVEN** a valid session user with `UserId` and `Name` exists
- **WHEN** the user invokes the logout action
- **THEN** the system redirects the browser to `/login`

#### Scenario: Logged-out user cannot re-enter the homepage without logging in again
- **GIVEN** the user has just logged out and no valid session user exists
- **WHEN** the user requests the homepage route
- **THEN** the system redirects the user to the login route
