/// MIT License © 2025 Martín Duhalde + ChatGPT

using FluentValidation;

namespace CarRental.UseCases.Cars.Update;

public class UpdateCarCommandValidator : AbstractValidator<UpdateCarCommand>
{
    public UpdateCarCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model must be at most 100 characters.");
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required.")
            .MaximumLength(50).WithMessage("Type must be at most 50 characters.");
    }
}
