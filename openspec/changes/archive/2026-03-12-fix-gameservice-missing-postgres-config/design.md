## Context

GameService currently throws an `InvalidOperationException` during startup when `ConnectionStrings:postgres` is absent. This is brittle for local development and Aspire-managed execution, where connection information may be supplied through service discovery, alternative configuration keys, or environment-provided defaults rather than a single hard-coded connection-string location.

The change affects backend startup and persistence registration only. Public endpoints and persistence semantics should remain unchanged. The main stakeholders are backend developers working on GameService and DevOps/orchestration maintainers who rely on predictable startup behavior across local and orchestrated environments.

## Goals / Non-Goals

**Goals:**
- Allow GameService persistence startup to resolve PostgreSQL configuration from supported runtime configuration sources.
- Preserve clear failure behavior when no usable PostgreSQL configuration is available.
- Keep persistence wiring explicit and testable.
- Avoid breaking API or persistence behavior after startup succeeds.

**Non-Goals:**
- Changing GameService HTTP contracts or persistence schema.
- Introducing a new database provider or replacing PostgreSQL.
- Reworking broader solution-wide configuration architecture beyond what is needed for GameService startup.

## Decisions

1. **Centralize PostgreSQL configuration resolution behind a single startup path**
   - **Decision:** Update GameService persistence registration to resolve PostgreSQL configuration through a single helper/path that can inspect supported configuration keys and orchestration-provided values.
   - **Rationale:** Reduces brittle assumptions and keeps startup validation consistent.
   - **Alternative considered:** Keep direct lookup of `ConnectionStrings:postgres` and document it better.
   - **Why not alternative:** Documentation alone does not fix failing local/orchestrated startup.

2. **Retain fail-fast behavior when configuration is truly missing**
   - **Decision:** Continue to stop startup if no valid persistence configuration can be resolved, but return a clearer actionable message.
   - **Rationale:** Prevents partially configured runtime behavior while improving diagnosability.
   - **Alternative considered:** Start GameService without persistence.
   - **Why not alternative:** Risks hidden runtime failures on create/list operations and diverges from current architecture expectations.

3. **Add focused tests for configuration resolution**
   - **Decision:** Cover supported configuration sources and missing-configuration failure behavior with targeted tests.
   - **Rationale:** Startup configuration bugs are easy to reintroduce and hard to diagnose without tests.
   - **Alternative considered:** Rely on manual local verification only.
   - **Why not alternative:** Insufficient regression protection.

## Risks / Trade-offs

- **[Risk] Multiple accepted configuration sources could hide precedence ambiguity** → **Mitigation:** Define deterministic precedence in code and tests.
- **[Risk] Clearer validation may still break previously misconfigured environments** → **Mitigation:** Use actionable error text that identifies accepted configuration inputs.
- **[Trade-off] Supporting orchestration-friendly resolution adds slight startup complexity** → **Mitigation:** Encapsulate the logic in a small helper/extension method.

## Migration Plan

1. Update GameService persistence registration to resolve PostgreSQL settings from supported configuration sources.
2. Add or update tests for successful resolution and missing-configuration failure.
3. Verify GameService startup in the intended local development/orchestration path.

Rollback:
- Revert GameService persistence registration changes.
- Revert associated tests if the new resolution strategy proves incompatible.

## Open Questions

- Which exact alternate configuration keys/resource bindings are already used by GameService under Aspire and local dev today?
- Should the startup error message enumerate all accepted configuration keys, or only the preferred one plus a reference to local setup guidance?
