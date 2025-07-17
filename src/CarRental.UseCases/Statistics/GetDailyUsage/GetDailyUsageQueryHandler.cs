/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.UseCases.Statistics.Dtos;

using MediatR;

namespace CarRental.UseCases.Statistics.GetDailyUsage;

/// <summary>
/// 📈 Query: Get daily stats with rentals, and unused cars for the last 7 days.
/// </summary>
public record GetDailyUsageQuery() : IRequest<List<DailyStatDto>>;

public class GetDailyUsageQueryHandler : IRequestHandler<GetDailyUsageQuery, List<DailyStatDto>>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;

    public GetDailyUsageQueryHandler(IRentalRepository rentalRepository, ICarRepository carRepository)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
    }

    public async Task<List<DailyStatDto>> Handle(GetDailyUsageQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var last7Days = Enumerable.Range(0, 7)
                                  .Select(offset => today.AddDays(-offset))
                                  .ToList();

        // Obtener todos los rentals de los últimos 7 días (incluye cancelados si es necesario)
        var rentals = await _rentalRepository.ListLast7DaysAsync(cancellationToken);
        
        // Obtener todos los autos
        var allCars = await _carRepository.ListAllActivesAsync(cancellationToken);

        var groupedByDay = rentals
            .GroupBy(r => r.StartDate.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        var data = last7Days.Select(day =>
        {
            var rentalsOfDay    /**/ = groupedByDay.ContainsKey(day) ? groupedByDay[day] : new List<Domain.Entities.Rental>();
            int cancellations   /**/ = rentalsOfDay.Count(r => r.RentalStatus == Domain.Entities.RentalStatus.Cancelled);
            int rentalsCount    /**/ = rentalsOfDay.Count - cancellations;

            // Autos no usados ese día (no rentados ni cancelados)
            int unusedCarsCount = allCars.Count - rentalsOfDay.Select(r => r.CarId).Distinct().Count();

            return new DailyStatDto
            {
                Date           /**/ = day,
                Rentals        /**/ = rentalsCount,
                Cancellations  /**/ = cancellations,
                UnusedCars     /**/ = unusedCarsCount < 0 ? 0 : unusedCarsCount
            };

        }).ToList();

        return data;
    }
}
