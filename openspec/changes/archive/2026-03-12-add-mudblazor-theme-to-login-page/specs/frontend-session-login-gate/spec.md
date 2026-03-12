## MODIFIED Requirements

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
