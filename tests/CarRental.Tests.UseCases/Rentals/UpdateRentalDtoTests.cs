/// MIT License © 2025 Martín Duhalde + ChatGPT

using FluentAssertions;
using CarRental.UseCases.Rentals.Dtos;

namespace CarRental.Tests.UseCases.Rentals;

public class UpdateRentalDtoTests
{
    [Fact]
    public void updateRentalDtoShould_hold_all_required_fields()
    {
        // Arrange
        var id         /**/ = Guid.NewGuid();
        var customerId /**/ = Guid.NewGuid();
        var carId      /**/ = Guid.NewGuid();
        var startDate  /**/ = new DateTime(2025, 7, 15, 10, 0, 0, DateTimeKind.Utc);
        var endDate    /**/ = new DateTime(2025, 7, 20, 10, 0, 0, DateTimeKind.Utc);
        var version    /**/ = 3;

        // Act
        var dto = new UpdateRentalDto
        {
            Id         /**/ = id,
            CustomerId /**/ = customerId,
            CarId      /**/ = carId,
            StartDate  /**/ = startDate,
            EndDate    /**/ = endDate,
            Version    /**/ = version
        };

        // Assert
        dto.Id.Should().Be(id);
        dto.CustomerId.Should().Be(customerId);
        dto.CarId.Should().Be(carId);
        dto.StartDate.Should().Be(startDate);
        dto.EndDate.Should().Be(endDate);
        dto.Version.Should().Be(version);
    }

    [Fact]
    public void updateRentalDtoShould_have_default_values_when_empty()
    {
        // Act
        var dto = new UpdateRentalDto();

        // Assert
        dto.Id.Should().Be(Guid.Empty);
        dto.CustomerId.Should().Be(Guid.Empty);
        dto.CarId.Should().Be(Guid.Empty);
        dto.StartDate.Should().Be(default);
        dto.EndDate.Should().Be(default);
        dto.Version.Should().Be(0);
    }
}
