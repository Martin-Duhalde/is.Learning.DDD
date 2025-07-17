namespace CarRental.UseCases.Common.Validators;
public class NotEmptyGuidAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is Guid guid && guid == Guid.Empty)
            return new ValidationResult($"{validationContext.MemberName} cannot be empty GUID.",
                                    new[] { validationContext.MemberName! }
    );

        return ValidationResult.Success;
    }
}