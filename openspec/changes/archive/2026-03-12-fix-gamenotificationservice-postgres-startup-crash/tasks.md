## 1. Persistence Initialization Flow

- [x] 1.1 Refactor GameNotificationService startup to remove the unconditional `ApplyNotificationMigrations()` crash path and introduce a dedicated persistence initializer.
- [x] 1.2 Add a notification persistence readiness state component that records initialization success/failure and exposes the latest availability state to the application.
- [x] 1.3 Implement bounded retry or reattempt logic so persistence initialization can recover automatically after PostgreSQL becomes reachable.

## 2. Runtime Behavior and Diagnostics

- [x] 2.1 Integrate persistence readiness with health checks so `/health` reports unhealthy dependency status instead of losing the process on startup failure.
- [x] 2.2 Update notification request handling to return a controlled service-unavailable response while notification persistence is not ready.
- [x] 2.3 Add structured logging and actionable error messages for initialization failures and recovery events.

## 3. Verification

- [x] 3.1 Add or update unit tests for initializer failure handling, readiness state transitions, and dependency-unavailable request behavior.
- [x] 3.2 Add or update integration tests covering startup with unavailable PostgreSQL, later recovery, and normal behavior once persistence is ready.
- [x] 3.3 Run the smallest relevant GameNotificationService build and test commands, then broader validation as needed to confirm the startup crash is resolved.
