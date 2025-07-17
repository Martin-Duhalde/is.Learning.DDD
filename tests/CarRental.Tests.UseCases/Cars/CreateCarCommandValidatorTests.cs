/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.UseCases.Cars.Create;

using FluentValidation.TestHelper;

namespace CarRental.Tests.UseCases.Cars;

public class CreateCarCommandValidatorTests
{
    private readonly CreateCarCommandValidator /**/ _validator = new();

    [Fact]
    public void should_pass_validation_with_valid_model_and_type()
    {
        // Arrange
        var command = new CreateCarCommand(
            Model: "Toyota Corolla",
            Type: "Sedan"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void should_fail_when_model_is_empty()
    {
        // Arrange
        var command = new CreateCarCommand(
            Model: "",
            Type: "SUV"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Model);
    }

    [Fact]
    public void should_fail_when_type_is_null()
    {
        // Arrange
        var command = new CreateCarCommand(
            Model: "Peugeot 208",
            Type: null!
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Type);
    }

   
    [Fact]
    public void should_fail_when_type_is_empty()
    {
        // Arrange
        var command = new CreateCarCommand(
            Model: "Nissan Versa",
            Type: ""
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Type);
    }
}
