/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.UseCases.Statistics.Dtos;

using MediatR;

namespace CarRental.UseCases.Statistics.GetTopCarsByBrandModel;

/// <summary>
/// 📊 Query: Get top rented cars ranked by brand, model and type between two dates.
/// </summary>
public record GetTopCarsByBrandModelQuery(DateTime From, DateTime To) : IRequest<List<TopCarByBrandModelDto>>;

public class GetTopCarsByBrandModelQueryHandler : IRequestHandler<GetTopCarsByBrandModelQuery, List<TopCarByBrandModelDto>>
{
    private readonly IRentalRepository _rentalRepository;

    public GetTopCarsByBrandModelQueryHandler(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<List<TopCarByBrandModelDto>> Handle(GetTopCarsByBrandModelQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepository.ListActivesBetweenDatesAsync(request.From, request.To, cancellationToken);

        var total = rentals.Count;
        if (total == 0) return new List<TopCarByBrandModelDto>();

        var grouped = rentals
          .GroupBy(r => new
          {
              Model = r.Car?.Model ?? "Unknown",
              Type = r.Car?.Type ?? "Unknown"
          })
          .Select(g => new TopCarByBrandModelDto
          {
              Model = g.Key.Model,
              Type = g.Key.Type,
              Count = g.Count(),
              Percentage = Math.Round((double)g.Count() / total * 100, 2)
          })
          .OrderByDescending(x => x.Count)
          .Take(10)
          .ToList();

        return grouped;
    }
}
