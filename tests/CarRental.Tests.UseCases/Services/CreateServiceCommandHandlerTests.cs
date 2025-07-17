/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Services.Create;

namespace CarRental.Tests.UseCases.Services;

public class CreateServiceCommandHandlerTests
{
    private readonly IServiceRepository _serviceRepository = Substitute.For<IServiceRepository>();
    private readonly CreateServiceCommandHandler _handler;

    public CreateServiceCommandHandlerTests()
    {
        _handler = new CreateServiceCommandHandler(_serviceRepository);
    }

    [Fact]
    public async Task should_create_service_when_no_existing_active_service_for_car_and_date()
    {
        // Arrange
        var command = new CreateServiceCommand(
            CarId: Guid.NewGuid(),
            Date: DateTime.UtcNow.AddDays(1)
        );

        _serviceRepository.FindActivesByCarAndDateAsync(command.CarId, command.Date, Arg.Any<CancellationToken>())
                          .Returns(new List<Service>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        await _serviceRepository.Received(1).AddAsync(Arg.Is<Service>(service =>
            service.CarId == command.CarId &&
            service.Date == command.Date &&
            service.IsActive == true &&
            service.Version == 1
        ), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task should_throw_if_duplicate_active_service_exists_for_car_and_date()
    {
        // Arrange
        var command = new CreateServiceCommand(
            CarId: Guid.NewGuid(),
            Date: DateTime.UtcNow
        );

        var existing = new Service { CarId = command.CarId, Date = command.Date, IsActive = true };
        _serviceRepository.FindActivesByCarAndDateAsync(command.CarId, command.Date, Arg.Any<CancellationToken>())
                          .Returns(new List<Service> { existing });

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Ya existe un servicio activo para este auto en la misma fecha.", ex.Message);
    }

    [Fact]
    public async Task should_throw_if_multiple_active_services_exist_for_car_and_date()
    {
        // Arrange
        var command = new CreateServiceCommand(
            CarId: Guid.NewGuid(),
            Date: DateTime.UtcNow
        );

        var existingServices = new List<Service>
        {
            new() { CarId = command.CarId, Date = command.Date, IsActive = true },
            new() { CarId = command.CarId, Date = command.Date, IsActive = true }
        };

        _serviceRepository.FindActivesByCarAndDateAsync(command.CarId, command.Date, Arg.Any<CancellationToken>())
                          .Returns(existingServices);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Inconsistencia: múltiples servicios activos registrados para el mismo auto y fecha.", ex.Message);
    }
}
