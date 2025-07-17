/// MIT License © 2025 Martín Duhalde + ChatGPT

using AutoMapper;

using CarRental.UseCases.Services.Dtos;

namespace CarRental.UseCases.Services.Create  
{
    public class CreateServiceMappingProfile : Profile
    {
        public CreateServiceMappingProfile()
        {
            CreateMap<CreateServiceRequestDto, CreateServiceCommand>();
            CreateMap<Guid, CreateServiceResponseDto>()
                .ConstructUsing(id => new CreateServiceResponseDto(id));
        }
    }
}
