## 1. API Contract and Routing

- [x] 1.1 Add unified status update endpoint in GameService (for example `PUT /api/games/{id}/status`) with request DTO containing status.
- [x] 1.2 Add validation rules to accept only `Active` and `Completed` and return consistent validation errors for invalid or missing status.
- [x] 1.3 Update FastEndpoints registration and Swagger metadata to document the new endpoint contract.

## 2. Domain and Persistence Updates

- [x] 2.1 Refactor status transition handling into a single service path used by the unified endpoint.
- [x] 2.2 Update PostgreSQL persistence logic to perform id-based status updates atomically and return not-found when game id does not exist.
- [x] 2.3 Add or update integration tests for Active and Completed transitions, invalid status handling, and unknown id behavior.

## 3. Backward Compatibility and Migration

- [x] 3.1 Implement temporary compatibility mapping for legacy Complete and Activate routes to call the unified status update path.
- [x] 3.2 Add deprecation telemetry/logging for legacy route usage and include migration guidance in API docs.
- [x] 3.3 Define and execute rollout verification checklist, including rollback steps to restore legacy route behavior if needed.
