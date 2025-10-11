/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Repositories;
using CarRental.Domain.Exceptions;

using MediatR;

namespace CarRental.Application.Rentals.Cancel;

public record CancelRentalCommand(Guid RentalId) : IRequest<Unit>;

public class CancelRentalCommandHandler : IRequestHandler<CancelRentalCommand, Unit>
{
    private readonly IRentalRepository _rentalRepository;

    public CancelRentalCommandHandler(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }
       
    public async Task<Unit> Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetActiveByIdAsync(request.RentalId, cancellationToken)
            ?? throw new DomainException("Rental not found.");

        await _rentalRepository.CancelAsync(request.RentalId, cancellationToken);

        return Unit.Value;
    }
}