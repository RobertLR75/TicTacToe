## 1. Configuration Resolution

- [x] 1.1 Inspect current GameService startup and persistence registration to identify the exact `ConnectionStrings:postgres` dependency path.
- [x] 1.2 Implement a centralized PostgreSQL configuration resolution path that supports the intended local/Aspire runtime configuration sources.
- [x] 1.3 Update startup validation so missing persistence configuration still fails fast with a clearer actionable error message.

## 2. Service Wiring

- [x] 2.1 Update `Program.cs` and/or persistence registration extensions to use the new configuration resolution path without changing API behavior.
- [x] 2.2 Preserve deterministic precedence when multiple supported PostgreSQL configuration sources are present.

## 3. Validation and Tests

- [x] 3.1 Add or update unit tests for successful configuration resolution from supported sources.
- [x] 3.2 Add or update tests for missing-configuration failure behavior and actionable error messaging.
- [x] 3.3 Run the smallest relevant GameService test/build commands and fix any regressions introduced by the startup change.

## 4. Runtime Verification

- [x] 4.1 Verify GameService can start in the intended local or Aspire-aligned configuration path without the original exception.
- [x] 4.2 Confirm no public API or persistence behavior changes beyond startup configuration handling.
