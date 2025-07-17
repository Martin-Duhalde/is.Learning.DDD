/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Cars.Delete;

using MediatR;

namespace CarRental.Tests.UseCases.Cars;

public class DeleteCarCommandHandlerTests
{
    private readonly ICarRepository            /**/ _carRepository /**/ = Substitute.For<ICarRepository>();
    private readonly DeleteCarCommandHandler   /**/ _handler;

    public DeleteCarCommandHandlerTests()
    {
        _handler = new DeleteCarCommandHandler(_carRepository);
    }

    [Fact]
    public async Task should_delete_car_when_it_exists_and_is_active()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var car = new Car { Id = carId, IsActive = true };

        _carRepository.GetActiveByIdAsync(carId, Arg.Any<CancellationToken>())
                      .Returns(car);

        // Act
        var result = await _handler.Handle(new DeleteCarCommand(carId), CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        await _carRepository.Received(1).DeleteAsync(car, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task should_throw_domain_exception_when_car_not_found()
    {
        // Arrange
        var carId = Guid.NewGuid();

        _carRepository.GetActiveByIdAsync(carId, Arg.Any<CancellationToken>())
                      .Returns((Car?)null);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(async () =>
            await _handler.Handle(new DeleteCarCommand(carId), CancellationToken.None)
        );

        Assert.Equal($"Car with ID {carId} not found.", ex.Message);
        await _carRepository.DidNotReceive().DeleteAsync(Arg.Any<Car>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task should_throw_domain_exception_when_car_exists_but_is_inactive()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var inactiveCar = new Car { Id = carId, IsActive = false };

        // Simulamos que GetActiveByIdAsync no retorna autos inactivos, por eso aquí devolvemos null:
        _carRepository.GetActiveByIdAsync(carId, Arg.Any<CancellationToken>())
                      .Returns((Car?)null);

        // Si en tu repo se busca con filtro IsActive = true, auto inactivo no se encuentra

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(async () =>
            await _handler.Handle(new DeleteCarCommand(carId), CancellationToken.None)
        );

        Assert.Equal($"Car with ID {carId} not found.", ex.Message);
        await _carRepository.DidNotReceive().DeleteAsync(Arg.Any<Car>(), Arg.Any<CancellationToken>());
    }
    [Fact]
    public async Task should_propagate_exception_if_delete_fails()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var car = new Car { Id = carId, IsActive = true };

        _carRepository.GetActiveByIdAsync(carId, Arg.Any<CancellationToken>())
                      .Returns(car);

        _carRepository.When(r => r.DeleteAsync(car, Arg.Any<CancellationToken>()))
                      .Do(x => throw new InvalidOperationException("Delete failed"));

        // Act + Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(new DeleteCarCommand(carId), CancellationToken.None)
        );

        Assert.Equal("Delete failed", ex.Message);
    }

    [Fact]
    public async Task should_not_call_delete_if_car_not_found()
    {
        // Arrange
        var carId = Guid.NewGuid();

        _carRepository.GetActiveByIdAsync(carId, Arg.Any<CancellationToken>())
                      .Returns((Car?)null);

        // Act + Assert
        await Assert.ThrowsAsync<DomainException>(async () =>
            await _handler.Handle(new DeleteCarCommand(carId), CancellationToken.None)
        );

        await _carRepository.DidNotReceive().DeleteAsync(Arg.Any<Car>(), Arg.Any<CancellationToken>());
    }
}
