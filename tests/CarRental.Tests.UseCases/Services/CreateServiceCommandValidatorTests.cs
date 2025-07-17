/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.UseCases.Cars.Create;
using CarRental.UseCases.Services.Create;

using FluentValidation.TestHelper;

using Xunit;

namespace CarRental.Tests.UseCases.Services;

public class CreateServiceCommandValidatorTests
{
    private readonly CreateServiceCommandValidator _validator = new();

    [Fact]
    public void should_pass_validation_with_valid_carid_and_date()
    {
        // Arrange
        var command = new CreateServiceCommand(
            CarId: Guid.NewGuid(),
            Date: DateTime.UtcNow.AddDays(1)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void should_fail_when_carid_is_empty()
    {
        // Arrange
        var command = new CreateServiceCommand(
            CarId: Guid.Empty,
            Date: DateTime.UtcNow.AddDays(1)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CarId);
    }

    [Fact]
    public void should_fail_when_date_is_empty_or_minvalue()
    {
        // Arrange
        var command1 = new CreateServiceCommand(
            CarId: Guid.NewGuid(),
            Date: DateTime.MinValue
        );
        var command2 = new CreateServiceCommand(
            CarId: Guid.NewGuid(),
            Date: default
        );

        // Act
        var result1 = _validator.TestValidate(command1);
        var result2 = _validator.TestValidate(command2);

        // Assert
        result1.ShouldHaveValidationErrorFor(c => c.Date);
        result2.ShouldHaveValidationErrorFor(c => c.Date);
    }
}
