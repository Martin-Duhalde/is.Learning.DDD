/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.UseCases.Rentals.Dtos;

using MediatR;

namespace CarRental.UseCases.Rentals.Get;

/// <summary>
/// 📊 Query to retrieve car rental statistics.
/// </summary>
/// <remarks>
/// Returns usage percentage and ranking by car type.
/// </remarks>
public record GetRentalStatsQuery(DateTime From, DateTime To) : IRequest<List<RentalStatDto>>;

public class GetRentalStatsQueryHandler : IRequestHandler<GetRentalStatsQuery, List<RentalStatDto>>
{
    private readonly IRentalRepository _rentalRepo;

    public GetRentalStatsQueryHandler(IRentalRepository rentalRepo)
    {
        _rentalRepo = rentalRepo;
    }

    public async Task<List<RentalStatDto>> Handle(GetRentalStatsQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepo.ListActivesBetweenDatesAsync(request.From, request.To, cancellationToken);

        var grouped = rentals
            .GroupBy(r => r.Car?.Type ?? "Unknown")
            .Select(g => new
            {
                Type = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        int total = grouped.Sum(x => x.Count);

        return grouped.Select(x => new RentalStatDto
        {
            CarType = x.Type,
            Count = x.Count,
            Percentage = total == 0 ? 0 : Math.Round((double)x.Count / total * 100, 2)
        }).ToList();
    }
}