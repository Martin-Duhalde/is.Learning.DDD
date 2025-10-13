/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Interfaces;
using CarRental.Application.Rentals.Dtos;

using MediatR;
using System.Linq;

namespace CarRental.Application.Rentals.CheckAvailability
{
    public record CheckAvailabilityQuery(DateTime StartDate, DateTime EndDate, string Type, string Model) : IRequest<List<CarAvailabilityDto>>;

    public class CheckAvailabilityQueryHandler : IRequestHandler<CheckAvailabilityQuery, List<CarAvailabilityDto>>
    {
        private readonly ICarAvailabilityReadService _availabilityService;

        public CheckAvailabilityQueryHandler(ICarAvailabilityReadService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        public async Task<List<CarAvailabilityDto>> Handle(CheckAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var cars = await _availabilityService.ListAvailableAsync(
                request.Type,
                request.Model,
                request.StartDate,
                request.EndDate,
                cancellationToken);

            return cars.Select(car => new CarAvailabilityDto
            {
                Id = car.Id,
                Model = car.Model,
                Type = car.Type
            }).ToList();
        }
    }
}
