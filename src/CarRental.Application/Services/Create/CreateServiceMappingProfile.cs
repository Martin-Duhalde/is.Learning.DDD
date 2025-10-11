/// MIT License © 2025 Martín Duhalde + ChatGPT

using AutoMapper;

using CarRental.Application.Services.Dtos;

namespace CarRental.Application.Services.Create  
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
