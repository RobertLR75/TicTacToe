# Mappers

Use `Mapper<TReq, TRes, TEntity>` for write operations (POST/PUT) and `ResponseMapper<TRes, TEntity>` for read operations (GET).

```csharp
// Write operation — maps both directions
public class CreatePersonMapper : Mapper<CreatePersonRequest, CreatePersonResponse, PersonModel>
{
    public override PersonModel ToEntity(CreatePersonRequest r) => new()
    {
        FirstName = r.FirstName.Trim(),
        LastName = r.LastName.Trim()
    };

    public override CreatePersonResponse FromEntity(PersonModel e) => new()
    {
        Id = e.Id,
        Name = $"{e.FirstName} {e.LastName}",
        CreatedDate = e.CreatedAt
    };
}

// Read operation — response mapping only
public class GetPersonMapper : ResponseMapper<GetPersonResponse, PersonModel>
{
    public override GetPersonResponse FromEntity(PersonModel e) => new()
    {
        Id = e.Id,
        Name = $"{e.FirstName} {e.LastName}",
        CreatedDate = e.CreatedAt
    };
}
```

- `ToEntity`: normalize input (trim strings, set defaults)
- `FromEntity`: transform domain model to API shape (combine fields, rename, format dates)
- Access via `Map.ToEntity(request)` / `Map.FromEntity(result)` in the endpoint