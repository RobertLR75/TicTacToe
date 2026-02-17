# Validators

Validators are nested classes inside their Request class — validation rules are part of the request's contract.

```csharp
public class CreatePersonRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public class CreatePersonValidator : Validator<CreatePersonRequest>
    {
        public CreatePersonValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(10);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(200);
        }
    }
}
```

- Naming: `{Action}Validator` nested inside `{Action}Request`
- Validators handle field-level rules only (required, length, format)
- Business logic validation (uniqueness, existence checks) belongs in the Command or service layer, not in validators
- Validators can inject services but prefer to keep them simple