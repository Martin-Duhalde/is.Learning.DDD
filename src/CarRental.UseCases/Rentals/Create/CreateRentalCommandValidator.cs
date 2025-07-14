/// MIT License © 2025 Martín Duhalde + ChatGPT

using FluentValidation;

namespace CarRental.UseCases.Rentals.Create;
public class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
{
    public CreateRentalCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId is required.");
        RuleFor(x => x.CarId).NotEmpty().WithMessage("CarId is required.");
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("StartDate must be before EndDate.");
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");
    }
}