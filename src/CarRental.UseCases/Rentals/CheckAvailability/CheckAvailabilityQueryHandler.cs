/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.UseCases.Rentals.Dtos;

using MediatR;

namespace CarRental.UseCases.Rentals.CheckAvailability
{
    public record CheckAvailabilityQuery(DateTime StartDate, DateTime EndDate, string Type, string Model) : IRequest<List<CarAvailabilityDto>>;

    public class CheckAvailabilityQueryHandler : IRequestHandler<CheckAvailabilityQuery, List<CarAvailabilityDto>>
    {
        private readonly ICarRepository _carRepository;

        public CheckAvailabilityQueryHandler(ICarRepository carRepo)
        {
            _carRepository = carRepo;
        }

        public async Task<List<CarAvailabilityDto>> Handle(CheckAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var allCars = await _carRepository.ListAllActivesAsync(cancellationToken);

            var filtered = allCars
                .Where(c => c.Type == request.Type && c.Model == request.Model)
                .ToList();

            var available = new List<CarAvailabilityDto>();

            foreach (var car in filtered)
            {
                var isAvailable = await _carRepository.IsAvailableAsync(car.Id, request.StartDate, request.EndDate, cancellationToken);
                if (isAvailable)
                {
                    available.Add(new CarAvailabilityDto
                    {
                        Id      /**/ = car.Id,
                        Model   /**/ = car.Model,
                        Type    /**/ = car.Type
                    });
                }
            }

            return available;
        }
    }
}
