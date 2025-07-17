/// MIT License © 2025 Martín Duhalde + ChatGPT

using FluentValidation;

namespace CarRental.UseCases.Services.Create;

public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.CarId)
            .NotEmpty()
            .WithMessage("CarId is required.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .Must(date => date > DateTime.MinValue)
            .WithMessage("Date is required.")
            .Must(date => date > DateTime.MinValue)
            .WithMessage("Date must be a valid date.");
    }
}
