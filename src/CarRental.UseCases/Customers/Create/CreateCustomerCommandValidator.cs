/// MIT License © 2025 Martín Duhalde + ChatGPT

using FluentValidation;

namespace CarRental.UseCases.Customers.Create
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(100).WithMessage("FullName must be at most 100 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address must be at most 200 characters.");
        }
    }
}
