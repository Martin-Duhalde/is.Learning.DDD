/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.UseCases.Cars.Update;

using FluentValidation.TestHelper;

namespace CarRental.Tests.UseCases.Cars;

public class UpdateCarCommandValidatorTests
{
    private readonly UpdateCarCommandValidator /**/ _validator = new();

    [Fact]
    public void should_pass_validation_with_valid_properties()
    {
        // Arrange
        var command = new UpdateCarCommand(
            Id: Guid.NewGuid(),
            Model: "Toyota Corolla",
            Type: "Sedan",
            Version: 1
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void should_fail_when_id_is_empty()
    {
        // Arrange
        var command = new UpdateCarCommand(
            Id: Guid.Empty,
            Model: "Toyota Corolla",
            Type: "Sedan",
            Version: 1
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id)
              .WithErrorMessage("Id is required.");
    }

    [Fact]
    public void should_fail_when_model_is_empty()
    {
        // Arrange
        var command = new UpdateCarCommand(
            Id: Guid.NewGuid(),
            Model: "",
            Type: "Sedan",
            Version: 1
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Model)
              .WithErrorMessage("Model is required.");
    }

    [Fact]
    public void should_fail_when_model_exceeds_max_length()
    {
        // Arrange
        var longModel = new string('A', 101); // 101 chars > 100 max
        var command = new UpdateCarCommand(
            Id: Guid.NewGuid(),
            Model: longModel,
            Type: "SUV",
            Version: 1
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Model)
              .WithErrorMessage("Model must be at most 100 characters.");
    }

    [Fact]
    public void should_fail_when_type_is_empty()
    {
        // Arrange
        var command = new UpdateCarCommand(
            Id: Guid.NewGuid(),
            Model: "Toyota Corolla",
            Type: "",
            Version: 1
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Type)
              .WithErrorMessage("Type is required.");
    }

    [Fact]
    public void should_fail_when_type_exceeds_max_length()
    {
        // Arrange
        var longType = new string('B', 51); // 51 chars > 50 max
        var command = new UpdateCarCommand(
            Id: Guid.NewGuid(),
            Model: "Toyota Corolla",
            Type: longType,
            Version: 1
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Type)
              .WithErrorMessage("Type must be at most 50 characters.");
    }
}
