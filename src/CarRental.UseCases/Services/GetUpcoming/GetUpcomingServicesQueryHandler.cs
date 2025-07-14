/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.UseCases.Rentals.Dtos;

using MediatR;

namespace CarRental.UseCases.Services.GetUpcoming;

public record GetUpcomingServicesQuery(DateTime From, DateTime To) : IRequest<List<UpcomingServiceDto>>;

public class GetUpcomingServicesQueryHandler : IRequestHandler<GetUpcomingServicesQuery, List<UpcomingServiceDto>>
{
    private readonly IServiceRepository _serviceRepo;

    public GetUpcomingServicesQueryHandler(IServiceRepository serviceRepo)
    {
        _serviceRepo = serviceRepo;
    }

    public async Task<List<UpcomingServiceDto>> Handle(GetUpcomingServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _serviceRepo.GetScheduledServicesAsync(request.From, request.To, cancellationToken);

        return services.Select(s => new UpcomingServiceDto
        {
            Model = s.Model,
            Type = s.Type,
            Date = s.Date
        }).ToList();
    }
}
