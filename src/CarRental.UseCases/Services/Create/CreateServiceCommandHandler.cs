/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;

using MediatR;

namespace CarRental.UseCases.Services.Create;

public record CreateServiceCommand(Guid CarId, DateTime Date) : IRequest<Guid>;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Guid>
{
    private readonly IServiceRepository _serviceRepository;

    public CreateServiceCommandHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        /// Recupera servicios existentes para ese mismo auto y fecha (sin incluir los eliminados lógicamente)
        var existingServices = await _serviceRepository.FindActivesByCarAndDateAsync(request.CarId, request.Date, cancellationToken);

        /// Control de integridad: si hay más de uno, el dominio está inconsistente
        if (existingServices.Count > 1)
            throw new DomainException("Inconsistencia: múltiples servicios activos registrados para el mismo auto y fecha.");

        /// Si ya existe uno, bloqueamos la creación (evitamos duplicados)
        if (existingServices.Any())
            throw new DomainException("Ya existe un servicio activo para este auto en la misma fecha.");

        /// Creación del nuevo servicio
        var service = new Service
        {
            Id = Guid.NewGuid(),
            CarId = request.CarId,
            Date = request.Date,
            IsActive = true,     /// Estado lógico por defecto
            Version = 1          /// Versión inicial
        };

        await _serviceRepository.AddAsync(service, cancellationToken);

        return service.Id;
    }
}
