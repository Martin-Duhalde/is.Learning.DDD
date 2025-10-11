// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Cars.Create;

using FluentAssertions;

namespace CarRental.Tests.Application.Cars.Dtos;

public class CreateCarResponseDtoTests
{
    [Fact]
    public void Should_Create_ResponseDto_With_Correct_CarId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();

        // Act
        var dto = new CreateCarResponseDto(expectedId);

        // Assert
        dto.CarId.Should().Be(expectedId);
    }

    [Fact]
    public void CarId_Should_Be_ReadOnly()
    {
        // Arrange
        var property = typeof(CreateCarResponseDto).GetProperty(nameof(CreateCarResponseDto.CarId));

        // Act & Assert
        property?.CanWrite.Should().BeFalse("CarId should be readonly");
    }
}
