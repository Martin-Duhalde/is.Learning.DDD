﻿/// MIT License © 2025 Martín Duhalde + ChatGPT

using FluentAssertions;
using CarRental.UseCases.Cars.Dtos;

namespace CarRental.Tests.UseCases.Cars.Dtos;

public class UpdateCarDtoTests
{
    [Fact]
    public void should_hold_id_model_type_and_version()
    {
        // Arrange
        var id      /**/ = Guid.NewGuid();
        var model   /**/ = "Ford Focus";
        var type    /**/ = "Sedan";
        var version /**/ = 2;

        // Act
        var dto = new UpdateCarDto
        {
            Id      /**/ = id,
            Model   /**/ = model,
            Type    /**/ = type,
            Version /**/ = version
        };

        // Assert
        dto.Id.Should().Be(id);
        dto.Model.Should().Be(model);
        dto.Type.Should().Be(type);
        dto.Version.Should().Be(version);
    }

    [Fact]
    public void should_have_default_values_when_empty()
    {
        // Act
        var dto = new UpdateCarDto();

        // Assert
        dto.Id.Should().Be(Guid.Empty);
        dto.Model.Should().BeEmpty();
        dto.Type.Should().BeEmpty();
        dto.Version.Should().Be(0);
    }
}
