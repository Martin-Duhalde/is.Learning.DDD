using CarRental.UseCases.Cars.Create;

using System.ComponentModel.DataAnnotations;

namespace CarRental.Tests.UseCases.Cars.Dtos;

public class CreateCarRequestDtoTests
{
    [Fact]
    public void should_pass_validation_when_model_and_type_are_valid()
    {
        var dto = new CreateCarRequestDto
        {
            Model = "Toyota Corolla",
            Type = "Sedan"
        };

        var context = new ValidationContext(dto, null, null);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void should_fail_validation_when_model_is_missing()
    {
        var dto = new CreateCarRequestDto
        {
            Model = "",
            Type = "SUV"
        };

        var context = new ValidationContext(dto, null, null);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Model is required.");
    }

    [Fact]
    public void should_fail_validation_when_type_is_too_long()
    {
        var dto = new CreateCarRequestDto
        {
            Model = "Ford",
            Type = new string('X', 51) // más de 50 caracteres
        };

        var context = new ValidationContext(dto, null, null);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Type length can't exceed 50 characters.");
    }
}
