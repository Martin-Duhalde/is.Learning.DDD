/// MIT License © 2025 Martín Duhalde + ChatGPT

using AutoMapper;

using CarRental.UseCases.Services.Create;
using CarRental.UseCases.Services.Dtos;

using Microsoft.Extensions.Logging;

namespace CarRental.Tests.UseCases.Services;

public class CreateServiceMappingProfileTests
{
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _config;

    public CreateServiceMappingProfileTests()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Configura AutoMapper y agrega el perfil
        _config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateServiceMappingProfile>();
        }, loggerFactory);

        // Valida la configuración
        _config.AssertConfigurationIsValid();
        
        // Crea el mapper
        _mapper = _config.CreateMapper();
    }

    [Fact]
    public void should_map_CreateServiceRequestDto_to_CreateServiceCommand()
    {
        // Arrange
        var dto = new CreateServiceRequestDto
        {
            CarId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(2)
        };

        // Act
        var command = _mapper.Map<CreateServiceCommand>(dto);

        // Assert
        Assert.Equal(dto.CarId, command.CarId);
        Assert.Equal(dto.Date, command.Date);
    }

    [Fact]
    public void should_map_Guid_to_CreateServiceResponseDto()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var responseDto = _mapper.Map<CreateServiceResponseDto>(id);

        // Assert
        Assert.Equal(id, responseDto.ServiceId);
    }
}
