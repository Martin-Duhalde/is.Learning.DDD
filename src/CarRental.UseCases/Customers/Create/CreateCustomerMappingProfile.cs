/// MIT License © 2025 Martín Duhalde + ChatGPT

using AutoMapper;

namespace CarRental.UseCases.Customers.Create
{
    public class CreateCustomerMappingProfile : Profile
    {
        public CreateCustomerMappingProfile()
        {
            CreateMap<CreateCustomerRequestDto, CreateCustomerCommand>();
            CreateMap<Guid, CreateCustomerResponseDto>()
                .ConstructUsing(id => new CreateCustomerResponseDto(id));
        }
    }
}
