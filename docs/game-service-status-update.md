# Game Service Status Update Migration

## Unified endpoint

- New endpoint: `PUT /api/game-lobby/{id}/status`
- Request body:

```json
{
  "status": "Active"
}
```

- Allowed status values: `Active`, `Completed`
- Invalid or missing values return a validation error response.

## Legacy compatibility routes (deprecated)

- `PUT /api/game-lobby/{id}/activate` -> internally maps to status `Active`
- `PUT /api/game-lobby/{id}/complete` -> internally maps to status `Completed`

Both legacy routes remain available for migration compatibility and emit deprecation logs.

## Rollout verification checklist

- [x] Unified endpoint returns success for `Active` and `Completed` requests.
- [x] Unified endpoint returns validation errors for unsupported statuses.
- [x] Unknown game id returns `404 Not Found` from unified and legacy routes.
- [x] PostgreSQL updates are applied by game id in `game_model`.
- [x] Deprecation warnings are visible in logs for legacy route usage.
- [x] Migration guidance for API clients is documented for unified endpoint adoption.

## Rollback plan

If migration causes client-impacting issues:

1. Prioritize legacy route traffic and communicate temporary rollback.
2. Keep `activate` and `complete` routes active as primary client guidance.
3. Revert unified endpoint adoption in dependent clients.
4. Redeploy the previous stable GameService image if required.
