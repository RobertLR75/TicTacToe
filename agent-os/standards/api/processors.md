# Processors

Processors run before (`IPreProcessor`) or after (`IPostProcessor`) an endpoint's `HandleAsync`. Two scopes:

- **Generic** (`Processors/` folder) — reusable across endpoints, typed on `<TRequest>` or `<TRequest, TResponse>`
- **Endpoint-specific** (operation folder) — typed on a concrete request, co-located with the endpoint

**Registration in `Configure()`:**
```csharp
public override void Configure()
{
    Put("/persons");
    PreProcessor<PreRequestLoggerProcessor<UpdatePersonRequest>>();  // generic
    PreProcessor<PreUpdatePersonRequestLogger>();                    // specific
    PostProcessor<PostResponseLoggerProcessor<UpdatePersonRequest, UpdatePersonResponse>>();
}
```

**Logger resolution pattern** (use in all processors — `ctx.HttpContext.Resolve<>()` throws in unit tests):
```csharp
public Task PreProcessAsync(IPreProcessorContext<TRequest> ctx, CancellationToken ct)
{
    var logger = ctx.HttpContext.RequestServices.GetService(typeof(ILogger<TRequest>)) as ILogger<TRequest>;

    if (logger is null)
    {
        try { logger = ctx.HttpContext.Resolve<ILogger<TRequest>>(); }
        catch (InvalidOperationException) { } // silent fail in tests
    }

    if (logger is not null)
        logger.LogInformation(...);

    return Task.CompletedTask;
}
```

- Always use `RequestServices.GetService()` first; `Resolve<>()` only as fallback
- Logging is best-effort — never throw if logger is unavailable