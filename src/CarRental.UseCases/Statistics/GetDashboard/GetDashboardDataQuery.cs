/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.UseCases.Statistics.Dtos;

using MediatR;

namespace CarRental.UseCases.Statistics.GetDashboard;

/// <summary>
/// 📊 Query to get dashboard summary data.
/// </summary>
public record GetDashboardDataQuery : IRequest<DashboardDto>;

public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, DashboardDto>
{
    private readonly IRentalRepository _rentalRepository;

    public GetDashboardDataQueryHandler(IRentalRepository rentalRepo)
    {
        _rentalRepository = rentalRepo;
    }

    public async Task<DashboardDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var last7Days = Enumerable.Range(0, 7)
            .Select(offset => today.AddDays(-offset))
            .ToList();

        var rentals = await _rentalRepository.ListLast7DaysAsync(cancellationToken);

        var grouped = rentals.GroupBy(r => r.StartDate.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        var data = last7Days.Select(day => new DailyStatDto
        {
            Date = day,
            Rentals = grouped.ContainsKey(day) ? grouped[day].Count : 0,
            //Cancellations = grouped.ContainsKey(day) ? grouped[day].Count(r => r.IsCancelled) : 0,
            UnusedCars = 0 // ← completar si tenés disponibilidad de autos por día

        }).ToList();

        return new DashboardDto
        {
            DailyStats = data
        };
    }
}