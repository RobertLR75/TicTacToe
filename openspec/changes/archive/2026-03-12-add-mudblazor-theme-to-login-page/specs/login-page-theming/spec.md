## ADDED Requirements

### Requirement: Login page uses MudBlazor themed layout
The system SHALL render the login route using MudBlazor components and the configured MudBlazor theme so the experience is visually consistent with the rest of the frontend.

#### Scenario: Login route renders themed container and form
- **GIVEN** no valid session user exists
- **WHEN** the user requests the login route
- **THEN** the system renders a MudBlazor-themed login layout with a visible title, username input, and primary submit action

### Requirement: Login page remains responsive across breakpoints
The system SHALL keep the themed login page usable on mobile and desktop viewports without horizontal overflow or clipped form controls.

#### Scenario: Mobile viewport layout remains usable
- **GIVEN** no valid session user exists
- **WHEN** the user opens the login route on a mobile-width viewport
- **THEN** the login form remains fully visible and interactable without horizontal scrolling

#### Scenario: Desktop viewport layout remains centered and readable
- **GIVEN** no valid session user exists
- **WHEN** the user opens the login route on a desktop-width viewport
- **THEN** the login form appears centered with readable spacing and typography

### Requirement: Themed login experience preserves accessibility basics
The system SHALL provide accessible visual and interaction states on the themed login page, including visible focus indicators and sufficient contrast for primary text and actions.

#### Scenario: Keyboard focus is visible on login controls
- **GIVEN** no valid session user exists
- **WHEN** the user navigates login controls using keyboard tab navigation
- **THEN** focused controls show a visible focus state

#### Scenario: Invalid username feedback remains perceivable
- **GIVEN** no valid session user exists
- **WHEN** the user submits an empty or whitespace-only username
- **THEN** the system shows a validation message that is visually distinguishable from surrounding text
