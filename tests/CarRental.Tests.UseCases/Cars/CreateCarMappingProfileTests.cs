/// MIT License © 2025 Martín Duhalde + ChatGPT

using AutoMapper;

using CarRental.UseCases.Cars.Create;

using Microsoft.Extensions.Logging;

namespace CarRental.Tests.UseCases.Cars;

public class CreateCarMappingProfileTests
{
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _config;

    public CreateCarMappingProfileTests()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Configura AutoMapper y agrega el perfil
        _config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateCarMappingProfile>();
        }, loggerFactory);


        // Valida la configuración
        _config.AssertConfigurationIsValid();

        // Crea el mapper
        _mapper = _config.CreateMapper();
    }

    [Fact]
    public void should_map_CreateCarRequestDto_to_CreateCarCommand()
    {
        // Arrange
        var dto = new CreateCarRequestDto
        {
            Model = "Toyota Corolla",
            Type = "Sedan"
        };

        // Act
        var command = _mapper.Map<CreateCarCommand>(dto);

        // Assert
        Assert.Equal(dto.Model, command.Model);
        Assert.Equal(dto.Type, command.Type);
    }

    [Fact]
    public void should_map_Guid_to_CreateCarResponseDto()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var responseDto = _mapper.Map<CreateCarResponseDto>(id);

        // Assert
        Assert.Equal(id, responseDto.CarId);
    }
}
