/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Cars.Create;

namespace CarRental.Tests.UseCases.Cars;

public class CreateCarCommandHandlerTests
{
    private readonly ICarRepository              /**/ _carRepository /**/ = Substitute.For<ICarRepository>();
    private readonly CreateCarCommandHandler     /**/ _handler;

    public CreateCarCommandHandlerTests()
    {
        _handler = new CreateCarCommandHandler(_carRepository);
    }

    [Fact]
    public async Task should_create_car_when_unique_model_and_type()
    {
        // Arrange
        var command = new CreateCarCommand(
            Model:  /**/ "Toyota Corolla",
            Type:   /**/ "Sedan"
        );

        _carRepository.FindByModelAndTypeAsync(command.Model, command.Type, Arg.Any<CancellationToken>())
                      .Returns(new List<Car>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        await _carRepository.Received(1).AddAsync(Arg.Is<Car>(car =>
            car.Model    /**/ == command.Model &&
            car.Type     /**/ == command.Type &&
            car.IsActive /**/ == true &&
            car.Version  /**/ == 1
        ), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task should_throw_if_duplicate_active_car_exists()
    {
        // Arrange
        var command = new CreateCarCommand(
            Model: /**/ "Honda Civic",
            Type:  /**/ "Sedan"
        );

        var existing = new Car { Model = command.Model, Type = command.Type, IsActive = true };
        _carRepository.FindByModelAndTypeAsync(command.Model, command.Type, Arg.Any<CancellationToken>())
                      .Returns(new List<Car> { existing });

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("A car with the same model and type already exists.", ex.Message);
    }

    [Fact]
    public async Task should_throw_if_multiple_cars_with_same_model_and_type_exist()
    {
        // Arrange
        var command = new CreateCarCommand(
            Model: /**/ "Ford Focus",
            Type:  /**/ "Hatchback"
        );

        var existingCars = new List<Car>
        {
            new() { Model = command.Model, Type = command.Type, IsActive = true },
            new() { Model = command.Model, Type = command.Type, IsActive = true }
        };

        _carRepository.FindByModelAndTypeAsync(command.Model, command.Type, Arg.Any<CancellationToken>())
                      .Returns(existingCars);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Data inconsistency: multiple cars with the same model and type exist.", ex.Message);
    }
  
}
