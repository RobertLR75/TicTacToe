# frontend-session-login-gate Specification

## Purpose
Define session-based login gating for the frontend so unauthenticated users are routed through login before accessing the homepage.

## Requirements

### Requirement: Homepage access requires session user identity
The system SHALL require a session user object containing `UserId` (GUID) and `Name` (string) before serving the homepage. If the session user is missing or invalid, the system MUST redirect the request to the login page.

#### Scenario: Redirect unauthenticated homepage request
- **GIVEN** no valid session user exists
- **WHEN** the user requests the homepage route
- **THEN** the system redirects the user to the login route

#### Scenario: Allow authenticated homepage request
- **GIVEN** a valid session user with `UserId` and `Name` exists
- **WHEN** the user requests the homepage route
- **THEN** the system serves the homepage without redirecting to login

### Requirement: Login submission creates session identity
The system SHALL provide a MudBlazor-themed login page that accepts a username. On valid submission, the system MUST create a session user with a new GUID `UserId` and submitted `Name`, persist it to session storage, and redirect the user to the homepage.

#### Scenario: Successful login creates session and redirects
- **GIVEN** no valid session user exists
- **WHEN** the user submits a non-empty username on the login page
- **THEN** the system creates a session user containing a generated GUID `UserId` and the submitted `Name`
- **THEN** the system redirects the user to the homepage

#### Scenario: Invalid login submission does not create session
- **GIVEN** no valid session user exists
- **WHEN** the user submits an empty or whitespace-only username on the login page
- **THEN** the system returns the login page with a validation error
- **THEN** the system does not create session user data

### Requirement: Login route behavior respects existing session
The system SHALL avoid unnecessary login prompts for already-authenticated users.

#### Scenario: Authenticated user visits login route
- **GIVEN** a valid session user with `UserId` and `Name` exists
- **WHEN** the user requests the login route
- **THEN** the system redirects the user to the homepage
