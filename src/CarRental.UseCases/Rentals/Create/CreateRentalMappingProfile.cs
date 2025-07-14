/// MIT License © 2025 Martín Duhalde + ChatGPT

using AutoMapper;

using CarRental.UseCases.Rentals.Dtos;

namespace CarRental.UseCases.Rentals.Create;

public class CreateRentalMappingProfile : Profile
{
    public CreateRentalMappingProfile()
    {
        CreateMap<CreateRentalRequestDto, CreateRentalCommand>();
        CreateMap<Guid, CreateRentalResponseDto>()
            .ConstructUsing(id => new CreateRentalResponseDto(id));
    }
}