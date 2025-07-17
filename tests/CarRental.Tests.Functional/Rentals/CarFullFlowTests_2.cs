///// MIT License © 2025 Martín Duhalde + ChatGPT

//using CarRental.UseCases.Cars.Create;
//using CarRental.UseCases.Cars.Dtos;
//using CarRental.UseCases.Cars.Update;

//using System.Net.Http.Headers;
//using System.Net.Http.Json;

//namespace CarRental.Tests.Functional.Cars;

//[Collection("Sequential")]
//public class CarFullFlowTests_2(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
//{
//    private readonly HttpClient _client = factory.CreateClient();

//    [Fact]
//    public async Task should_complete_full_car_crud_flow_via_api_only()
//    {
//        // ---------- 1. Crear un nuevo auto ----------
//        var createCommand = new CreateCarCommand
//        (
//            Model: "ModelS",
//            Type: "Sedan"
//        );

//        var createResponse = await _client.PostAsJsonAsync("/api/car", createCommand);
//        createResponse.EnsureSuccessStatusCode();

//        var createdCar = await createResponse.Content.ReadFromJsonAsync<CarDto>();
//        Assert.NotNull(createdCar);
//        Assert.NotEqual(Guid.Empty, createdCar.Id);
//        Assert.Equal("ModelS", createdCar.Model);
//        Assert.Equal("Sedan", createdCar.Type);

//        var carId = createdCar.Id;

//        // ---------- 2. Consultar lista de autos ----------
//        var listResponse = await _client.GetAsync("/api/car");
//        listResponse.EnsureSuccessStatusCode();

//        var cars = await listResponse.Content.ReadFromJsonAsync<List<CarDto>>();
//        Assert.NotNull(cars);
//        Assert.Contains(cars, c => c.Id == carId && c.Model == "ModelS" && c.Type == "Sedan");

//        // ---------- 3. Consultar auto por Id ----------
//        var getResponse = await _client.GetAsync($"/api/car/{carId}");
//        getResponse.EnsureSuccessStatusCode();

//        var carDetail = await getResponse.Content.ReadFromJsonAsync<CarDto>();
//        Assert.NotNull(carDetail);
//        Assert.Equal(carId, carDetail.Id);
//        Assert.Equal("ModelS", carDetail.Model);
//        Assert.Equal("Sedan", carDetail.Type);

//        // ---------- 4. Actualizar auto ----------
//        var updateCommand = new UpdateCarCommand
//        (
//            Id: carId,
//            Model: "ModelX",
//            Type: "SUV",
//            Version: carDetail.Version + 1  // Incrementar versión para la actualización
//        );

//        var updateResponse = await _client.PutAsJsonAsync($"/api/car/{carId}", updateCommand);
//        Assert.Equal(System.Net.HttpStatusCode.NoContent, updateResponse.StatusCode);

//        // ---------- 5. Verificar actualización ----------
//        var updatedResponse = await _client.GetAsync($"/api/car/{carId}");
//        updatedResponse.EnsureSuccessStatusCode();

//        var updatedCar = await updatedResponse.Content.ReadFromJsonAsync<CarDto>();
//        Assert.NotNull(updatedCar);
//        Assert.Equal(carId, updatedCar.Id);
//        Assert.Equal("ModelX", updatedCar.Model);
//        Assert.Equal("SUV", updatedCar.Type);

//        // ---------- 6. Eliminar auto ----------
//        var deleteResponse = await _client.DeleteAsync($"/api/car/{carId}");
//        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);

//        // ---------- 7. Verificar eliminación ----------
//        var deletedResponse = await _client.GetAsync($"/api/car/{carId}");
//        Assert.Equal(System.Net.HttpStatusCode.NotFound, deletedResponse.StatusCode);
//    }
//}
