/// MIT License © 2025 Martín Duhalde + ChatGPT

/// Este archivo está Normalizado

using CarRental.Tests.Functional.Common;
using CarRental.UseCases.Cars.Create;
using CarRental.UseCases.Cars.Dtos;

using FluentAssertions;

using System.Net;
using System.Net.Http.Json;

namespace CarRental.Tests.Functional.Cars;

/// <summary>
/// 🌐 Test funcional completo y profesional del endpoint `/api/car`.
///    Verifica: Alta, duplicados, errores y consulta, manteniendo limpieza entre ejecuciones.
/// </summary>
public class CarApiFlowTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient         /**/ client;
    private readonly IServiceProvider   /**/ serviceProvider;

    public CarApiFlowTests(CustomWebApplicationFactory<Program> factory)
    {
        client              /**/ = factory.CreateClient();
        serviceProvider     /**/ = factory.Services;
    }

    [Fact(DisplayName = "❤️ should return 200 on /alive")]
    public async Task should_return_200_on_alive()
    {
        var response = await client.GetAsync("/alive");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "✅ Full car flow: create, duplicate, get all, fail on bad input")]
    public async Task should_run_full_car_flow()
    {
        var dto = new CreateCarRequestDto { Model = "Ford Focus", Type = "Hatchback" };

        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto.Model, dto.Type);

        // ❤️ /alive
        var alive = await client.GetAsync("/alive");
        alive.StatusCode.Should().Be(HttpStatusCode.OK);

        // ➕ create
        var create = await client.PostAsJsonAsync("/api/car", dto);
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        // 🚫 duplicate
        var duplicate = await client.PostAsJsonAsync("/api/car", dto);
        duplicate.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // ✅ get all
        var all = await client.GetAsync("/api/car");
        all.StatusCode.Should().Be(HttpStatusCode.OK);
        var cars = await all.Content.ReadFromJsonAsync<List<CarDto>>();
        cars.Should().Contain(c => c.Model == dto.Model && c.Type == dto.Type);

        // 🚫 invalid
        var invalid = await client.PostAsJsonAsync("/api/car", new CreateCarRequestDto { Model = "", Type = "" });
        invalid.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "✅ Full car flow: create Tesla, verify, cleanup")]
    public async Task should_run_full_car_flow_tesla()
    {
        var dto = new CreateCarRequestDto { Model = "Tesla X", Type = "SUV" };

        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto.Model, dto.Type);

        var response = await client.PostAsJsonAsync("/api/car", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var all = await client.GetAsync("/api/car");
        all.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await all.Content.ReadFromJsonAsync<List<CarDto>>();
        list.Should().Contain(c => c.Model == dto.Model && c.Type == dto.Type);

        // Final cleanup
        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto.Model, dto.Type);
    }

    [Fact(DisplayName = "➕ should create a car (Toyota) successfully")]
    public async Task should_create_a_car_toyota_successfully()
    {
        var dto = new CreateCarRequestDto { Model = "Toyota Corolla", Type = "Sedan" };

        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto.Model, dto.Type);

        var response = await client.PostAsJsonAsync("/api/car", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "➕ should create car and return valid Id")]
    public async Task should_create_car_and_return_valid_id()
    {
        var dto = new CreateCarRequestDto { Model = "Tesla Model 3", Type = "Sedan" };

        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto.Model, dto.Type);

        var response = await client.PostAsJsonAsync("/api/car", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<CreateCarResponseDto>();
        created!.CarId.Should().NotBe(Guid.Empty);
    }

    [Fact(DisplayName = "⚫ should retrieve car by ID")]
    public async Task should_retrieve_car_by_id()
    {
        var dto = new CreateCarRequestDto { Model = "BMW X3", Type = "SUV" };

        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto.Model, dto.Type);

        var createResponse = await client.PostAsJsonAsync("/api/car", dto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<CreateCarResponseDto>();

        var getResponse = await client.GetAsync($"/api/car/{created!.CarId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var car = await getResponse.Content.ReadFromJsonAsync<CarDto>();
        car!.Model.Should().Be(dto.Model);
        car.Type.Should().Be(dto.Type);
        //car.IsActive.Should().BeTrue();
        car.Version.Should().Be(1);
        //car.Version.Should().BeGreaterThan(0);
        //car.Version.Should().Be(expectedVersion);
    }

    [Fact(DisplayName = "⚫ should get all cars")]
    public async Task should_get_all_cars()
    {
        var dto1 = new CreateCarRequestDto { Model = "Test Model 1", Type = "Sedan" };
        var dto2 = new CreateCarRequestDto { Model = "Test Model 2", Type = "SUV" };

        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto1.Model, dto1.Type);
        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto2.Model, dto2.Type);

        await client.PostAsJsonAsync("/api/car", dto1);
        await client.PostAsJsonAsync("/api/car", dto2);

        var response = await client.GetAsync("/api/car");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var cars = await response.Content.ReadFromJsonAsync<List<CarDto>>();
        cars.Should().Contain(c => c.Model == dto1.Model && c.Type == dto1.Type);
        cars.Should().Contain(c => c.Model == dto2.Model && c.Type == dto2.Type);
    }

    [Fact(DisplayName = "⚫ should return only active cars")]
    public async Task should_return_only_active_cars()
    {
        var activeDto = new CreateCarRequestDto { Model = "ActiveCar", Type = "Sedan" };
        var deletedDto = new CreateCarRequestDto { Model = "DeletedCar", Type = "SUV" };

        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, activeDto.Model, activeDto.Type);
        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, deletedDto.Model, deletedDto.Type);

        // Crear ambos
        var active = await client.PostAsJsonAsync("/api/car", activeDto);
        var deleted = await client.PostAsJsonAsync("/api/car", deletedDto);
        var deletedCar = await deleted.Content.ReadFromJsonAsync<CreateCarResponseDto>();

        // Eliminar uno
        await client.DeleteAsync($"/api/car/{deletedCar!.CarId}");

        // Obtener todos
        var getAll = await client.GetAsync("/api/car");
        getAll.StatusCode.Should().Be(HttpStatusCode.OK);

        var cars = await getAll.Content.ReadFromJsonAsync<List<CarDto>>();
        cars.Should().Contain(c => c.Model == activeDto.Model);
        cars.Should().NotContain(c => c.Model == deletedDto.Model);
    }

    [Fact(DisplayName = "🚫 should fail on duplicate car")]
    public async Task should_fail_on_duplicate_car()
    {
        var dto = new CreateCarRequestDto { Model = "Nissan Leaf", Type = "Hatchback" };

        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto.Model, dto.Type);

        await client.PostAsJsonAsync("/api/car", dto);
        var duplicate = await client.PostAsJsonAsync("/api/car", dto);

        duplicate.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "🚫 should fail on invalid car data")]
    public async Task should_fail_on_invalid_data()
    {
        var invalidDto = new CreateCarRequestDto { Model = "", Type = "" };
        var response = await client.PostAsJsonAsync("/api/car", invalidDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// 🚫 Ensures that an outdated row version fails to update a car record.
    /// </summary>
    /// <remarks>
    /// This test simulates an optimistic concurrency control scenario:
    /// - A car is created and fetched to retrieve its original <c>row version</c>.
    /// - A first update is performed using the correct version — ✅ succeeds with <c>204 NoContent</c>.
    /// - A second update attempts to reuse the same (now outdated) version — ❌ must fail with <c>409 Conflict</c>.
    /// </remarks>
    /// <expected>
    ///     🧪 PUT /api/car/{id}
    ///     ✅ First update: 204 NoContent — successful update
    ///     ❌ Second update: 409 Conflict — concurrency violation
    /// </expected>
    /// <seealso>
    /// This test validates the concurrency mechanism using RowVersion (ETag-like behavior).
    /// </seealso>
    [Fact(DisplayName = "🔁 should fail to update car with outdated row version")]
    public async Task should_fail_to_update_car_with_outdated_row_version()
    {
        var dto = new CreateCarRequestDto { Model = "Audi A4", Type = "Sedan" };

        // Ensure no preexisting data for a clean test run
        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto.Model, dto.Type);

        // Create a new car
        var create = await client.PostAsJsonAsync("/api/car", dto);
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var car = await create.Content.ReadFromJsonAsync<CreateCarResponseDto>();

        // Fetch current RowVersion
        // Simular una actualización válida para obtener la row version actual
        var get = await client.GetAsync($"/api/car/{car!.CarId}");
        var full = await get.Content.ReadFromJsonAsync<CarDto>();
        var originalVersion = full!.Version;

        // ✅ First update with correct RowVersion
        var update1 = new
        {
            Id = car.CarId,
            Model = "Audi A4 - Updated",
            Type = "Sedan",
            Version = originalVersion
        };

        // Expected result for successful update:
        //     204 NoContent.
        //     ✅ Update succeeded: the entity was modified using the correct and current row version.
        var res1 = await client.PutAsJsonAsync($"/api/car/{update1.Id}", update1);
        res1.StatusCode.Should().Be(HttpStatusCode.NoContent);  /// NoContent: Is OK and no data returned.

        // ❌ Second update with stale RowVersion
        // Segundo update con la row version anterior => debe fallar
        var update2 = new
        {
            Id = car.CarId,
            Model = "Audi A4 - Conflict",
            Type = "Sedan",
            Version = originalVersion
        };

        // Espected result for Concurrency conflict:
        //     409 Conflict. 
        //     🔄 Concurrency conflict: the entity was modified by another user or process.
        var res2 = await client.PutAsJsonAsync($"/api/car/{update1.Id}", update2);
        res2.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact(DisplayName = "❌ should soft delete car")]
    public async Task should_soft_delete_car()
    {
        var dto = new CreateCarRequestDto { Model = "Mazda CX-5", Type = "Crossover" };

        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto.Model, dto.Type);

        var create = await client.PostAsJsonAsync("/api/car", dto);
        var created = await create.Content.ReadFromJsonAsync<CreateCarResponseDto>();

        var delete = await client.DeleteAsync($"/api/car/{created!.CarId}");
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Intentar obtener el auto eliminado lógicamente
        var get = await client.GetAsync($"/api/car/{created.CarId}");
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "❌ should soft delete car and prevent future access")]
    public async Task should_soft_delete_car_and_prevent_future_access()
    {
        var dto = new CreateCarRequestDto { Model = "Mazda CX-5", Type = "Crossover" };

        await TestDataHelper.ClearCarsByModelAndTypeAsync(serviceProvider, dto.Model, dto.Type);

        var create = await client.PostAsJsonAsync("/api/car", dto);
        create.StatusCode.Should().Be(HttpStatusCode.Created); /// Expected result: 201 Created
        var created = await create.Content.ReadFromJsonAsync<CreateCarResponseDto>();

        /// Act: Send DELETE request to logically (soft) delete the car

        var delete = await client.DeleteAsync($"/api/car/{created!.CarId}");

        /// Expected result for successful logical deletion:
        ///     204 NoContent.
        ///     🗑️ Entity marked as deleted (soft delete) and excluded from public queries.
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        /// Try to access the soft-deleted car (should be invisible to consumers)
        var get = await client.GetAsync($"/api/car/{created.CarId}");

        /// Expected result for accessing a soft-deleted entity:
        ///     404 NotFound.
        ///     🚫 Resource not available: the entity is logically deleted.
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}
