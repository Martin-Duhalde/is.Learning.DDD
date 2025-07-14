/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.UseCases.Statistics.Dtos;

using MediatR;

namespace CarRental.UseCases.Statistics.GetTopCarTypes;

/// <summary>
/// 📊 Query: Get top rented car types between two dates.
/// </summary>
public record GetTopCarTypesQuery(DateTime From, DateTime To) : IRequest<List<TopCarTypeDto>>;

public class GetTopCarTypesQueryHandler : IRequestHandler<GetTopCarTypesQuery, List<TopCarTypeDto>>
{
    private readonly IRentalRepository  /**/ _rentalRepository;
    private readonly ICarRepository     /**/ _carRepository;

    public GetTopCarTypesQueryHandler(IRentalRepository rentalRepo, ICarRepository carRepo)
    {
        _rentalRepository   /**/ = rentalRepo;
        _carRepository      /**/ = carRepo;
    }

    public async Task<List<TopCarTypeDto>> Handle(GetTopCarTypesQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepository.ListActivesBetweenDatesAsync(request.From, request.To, cancellationToken);

        var total = rentals.Count;
        if (total == 0) return new List<TopCarTypeDto>();

        var grouped = rentals
            .GroupBy(r => r.Car?.Type ?? "Unknown")
            .Select(g => new TopCarTypeDto
            {
                Type = g.Key,
                Count = g.Count(),
                Percentage = Math.Round((double)g.Count() / total * 100, 2)
            })
            .OrderByDescending(x => x.Count)
            .Take(3)
            .ToList();

        return grouped;
    }
}