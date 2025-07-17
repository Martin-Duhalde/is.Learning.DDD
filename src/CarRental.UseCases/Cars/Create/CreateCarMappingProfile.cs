/// MIT License © 2025 Martín Duhalde + ChatGPT

using AutoMapper;

namespace CarRental.UseCases.Cars.Create;

public class CreateCarMappingProfile : Profile
{
    public CreateCarMappingProfile()
    {
        CreateMap<CreateCarRequestDto, CreateCarCommand>();
        CreateMap<Guid, CreateCarResponseDto>()
            .ConstructUsing(id => new CreateCarResponseDto(id));
    }
}
