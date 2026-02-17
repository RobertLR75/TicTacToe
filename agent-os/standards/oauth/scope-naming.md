# API Scope Naming Convention

Format: `{ServiceName}{Permission}`

- `ServiceName` — matches the API/service project name (e.g. ArrangementService → `Arrangement`)
- `Permission` — always `Read` or `Write`, no other variants

**Examples:**
```
ArrangementRead, ArrangementWrite
NotificationRead, NotificationWrite
GameRead, GameWrite
```

Every service gets exactly two scopes: one Read, one Write.
