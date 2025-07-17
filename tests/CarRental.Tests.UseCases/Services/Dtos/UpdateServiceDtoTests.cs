/// MIT License © 2025 Martín Duhalde + ChatGPT

using FluentAssertions;
using CarRental.UseCases.Services.Dtos;

namespace CarRental.Tests.UseCases.Services.Dtos;

public class UpdateServiceDtoTests
{
    [Fact]
    public void should_hold_id_date_and_car_id()
    {
        // Arrange
        var id     /**/ = Guid.NewGuid();
        var date   /**/ = new DateTime(2025, 6, 1, 8, 30, 0, DateTimeKind.Utc);
        var carId  /**/ = Guid.NewGuid();

        // Act
        var dto = new UpdateServiceDto
        {
            Id     /**/ = id,
            Date   /**/ = date,
            CarId  /**/ = carId
        };

        // Assert
        dto.Id.Should().Be(id);
        dto.Date.Should().Be(date);
        dto.CarId.Should().Be(carId);
    }

    [Fact]
    public void should_have_default_values_when_empty()
    {
        // Act
        var dto = new UpdateServiceDto();

        // Assert
        dto.Id.Should().Be(Guid.Empty);
        dto.Date.Should().Be(default);
        dto.CarId.Should().Be(Guid.Empty);
    }
}
