/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.UseCases.Services.Create;
using CarRental.UseCases.Services.Dtos;

using FluentAssertions;

using System.ComponentModel.DataAnnotations;

namespace CarRental.Tests.UseCases.Services.Dtos;

public class CreateServiceRequestDtoTests
{
    [Fact]
    public void should_be_valid_when_carId_is_not_empty()
    {
        // Arrange
        var dto = new CreateServiceRequestDto
        {
            Date   /**/ = DateTime.UtcNow,
            CarId  /**/ = Guid.NewGuid() // 👈 válido
        };

        // Act
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty(); // No errores de validación
    }

    [Fact]
    public void should_hold_date_and_carId()
    {
        // Arrange
        var date   /**/ = new DateTime(2025, 6, 1, 8, 30, 0, DateTimeKind.Utc);
        var carId  /**/ = Guid.NewGuid();

        // Act
        var dto = new CreateServiceRequestDto
        {
            Date   /**/ = date,
            CarId  /**/ = carId
        };

        // Assert
        dto.Date.Should().Be(date);
        dto.CarId.Should().Be(carId);
    }

    [Fact]
    public void should_have_default_values_when_empty()
    {
        // Act
        var dto = new CreateServiceRequestDto();

        // Assert
        dto.Date.Should().Be(default);
        dto.CarId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void should_be_invalid_when_date_is_default_in_command()
    {
        // Arrange
        var command = new CreateServiceCommand(CarId: Guid.NewGuid(), Date: default);

        var validator = new CreateServiceCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void should_be_invalid_when_carId_is_empty()
    {
        // Arrange
        var dto = new CreateServiceRequestDto
        {
            Date   /**/ = DateTime.UtcNow,
            CarId  /**/ = Guid.Empty
        };

        // Act
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains(nameof(CreateServiceRequestDto.CarId)));
    }
}
