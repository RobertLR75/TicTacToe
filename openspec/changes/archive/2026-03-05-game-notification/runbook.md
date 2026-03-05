# Game Notification Rollout and Rollback Runbook

## Compatibility Verification

- Route remains `GET /api/notifications`.
- Response remains JSON array (`application/json`).
- Empty-store behavior remains `200 OK` with `[]`.

## Rollout

1. Deploy migration-enabled `GameNotificationService` build.
2. Verify startup migration completed in logs.
3. Keep `Messaging:EnableEventConsumers=false` for initial smoke checks.
4. Validate `GET /api/notifications` returns `200 OK`.
5. Enable `Messaging:EnableEventConsumers=true`.
6. Confirm `GameCreated` and `GameStateUpdated` messages persist to `notifications`.
7. Monitor health endpoint and ingestion logs for payload validation warnings.

## Rollback

1. Set `Messaging:EnableEventConsumers=false` to stop ingestion immediately.
2. Keep API online for read-only access to previously persisted notifications.
3. If needed, roll back service deployment to previous version.
4. Preserve notification table data for investigation and replay planning.
