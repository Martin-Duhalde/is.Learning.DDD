/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Cars.Update;

using MediatR;

namespace CarRental.Tests.UseCases.Cars;

public class UpdateCarCommandHandlerTests
{
    private readonly ICarRepository             /**/ _carRepository /**/ = Substitute.For<ICarRepository>();
    private readonly UpdateCarCommandHandler    /**/ _handler;

    public UpdateCarCommandHandlerTests()
    {
        _handler = new UpdateCarCommandHandler(_carRepository);
    }

    [Fact]
    public async Task should_update_car_when_it_exists_and_version_matches()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var existingCar = new Car
        {
            Id = carId,
            Model = "Old Model",
            Type = "Old Type",
            Version = 3,
            IsActive = true
        };

        var command = new UpdateCarCommand(carId, "New Model", "New Type", 3);

        _carRepository.GetActiveByIdAsync(carId, Arg.Any<CancellationToken>())
                      .Returns(existingCar);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        Assert.Equal("New Model", existingCar.Model);
        Assert.Equal("New Type", existingCar.Type);

        await _carRepository.Received(1).UpdateAsync(existingCar, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task should_throw_if_car_not_found()
    {
        // Arrange
        var command = new UpdateCarCommand(Guid.NewGuid(), "Model", "Type", 1);

        _carRepository.GetActiveByIdAsync(command.Id, Arg.Any<CancellationToken>())
                      .Returns((Car?)null);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Car not found.", ex.Message);

        await _carRepository.DidNotReceive().UpdateAsync(Arg.Any<Car>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task should_throw_if_version_mismatch()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var existingCar = new Car
        {
            Id = carId,
            Model = "Model",
            Type = "Type",
            Version = 5,
            IsActive = true
        };

        var command = new UpdateCarCommand(carId, "Model Updated", "Type Updated", 3); // Version mismatch

        _carRepository.GetActiveByIdAsync(carId, Arg.Any<CancellationToken>())
                      .Returns(existingCar);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<ConcurrencyConflictException>(() =>
            _handler.Handle(command, CancellationToken.None));

        //throw new ConcurrencyConflictException("The car has been modified by another user or process.");
        Assert.Equal("The car has been modified by another user or process.", ex.Message);

        await _carRepository.DidNotReceive().UpdateAsync(Arg.Any<Car>(), Arg.Any<CancellationToken>());
    }
}
