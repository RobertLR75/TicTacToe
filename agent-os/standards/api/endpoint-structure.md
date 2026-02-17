# Endpoint Folder Structure

Each operation gets its own folder under `Endpoints/{Entity}/{Operation}/`.

```
Endpoints/
└── Persons/
    ├── Create/
    │   ├── CreatePersonEndpoint.cs
    │   ├── CreatePersonRequest.cs      # includes nested Validator
    │   ├── CreatePersonResponse.cs
    │   ├── CreatePersonMapper.cs
    │   └── PersonCreatedEvent.cs       # includes nested Handler
    ├── Get/
    ├── GetAll/
    └── Update/
        ├── UpdatePersonCommand.cs      # includes nested CommandHandler
        └── ...
```

- All files for an operation are co-located in its folder
- Naming: `{Action}{Entity}{FileType}.cs` (e.g. `CreatePersonEndpoint.cs`)
- Shared/generic files live outside: `Processors/` for generic processors, `Services/` for domain models