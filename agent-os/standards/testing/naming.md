# Test Naming

Test method names communicate the scenario and expected result without requiring code inspection.

**Pattern:** `{Method}_{Condition}_{ExpectedOutcome}` (or a readable subset)

```csharp
// Good — scenario and outcome are clear
[Fact] public async Task CreateAsync_WhenIdEmpty_AssignsGuidAndStoresEntity() { }
[Fact] public async Task DuplicateName_Fails() { }
[Fact] public async Task UniqueName_Passes() { }
[Fact] public async Task GetAsync_Returns404_WhenNotFound() { }

// Avoid — tells you nothing about what is tested or expected
[Fact] public async Task Test1() { }
[Fact] public async Task CreatePersonTest() { }
```

- Test class: `{ComponentName}Tests` (e.g. `CreatePersonEndpointTests`, `GetPersonMapperTests`)
- Use FluentAssertions — no raw `Assert` statements
- Use `TestContext.Current.CancellationToken` instead of `CancellationToken.None`