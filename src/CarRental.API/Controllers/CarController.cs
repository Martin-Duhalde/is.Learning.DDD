using AutoMapper;

using CarRental.Application.Cars.Create;
using CarRental.Application.Cars.Delete;
using CarRental.Application.Cars.Dtos;
using CarRental.Application.Cars.GetAll;
using CarRental.Application.Cars.GetById;
using CarRental.Application.Cars.Update;

using MediatR;

namespace CarRental.API.Controllers;

/// <summary>
/// 🎮 Controlador HTTP para operaciones CRUD sobre <c>Car</c>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CarController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor que recibe las dependencias necesarias.
    /// </summary>
    /// <param name="mediator">Instancia de MediatR para enviar comandos y queries.</param>
    /// <param name="mapper">Instancia de AutoMapper para traducir entre DTOs y comandos.</param>
    public CarController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 🔍 Obtiene el listado completo de autos.
    /// </summary>
    /// <returns>Lista de autos en formato JSON.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCars()
    {
        var query = new ListAllCarsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 🔍 Obtiene los detalles de un auto por su <c>Id</c>.
    /// </summary>
    /// <param name="id">Identificador del auto.</param>
    /// <returns>Objeto del auto encontrado o <c>404</c> si no existe.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCarById(Guid id)
    {
        var query = new GetCarByIdQuery(id);
        var car = await _mediator.Send(query);

        if (car is null)
            return NotFound();

        return Ok(car);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 🚗 Crea un nuevo auto.
    /// </summary>
    /// <param name="request">DTO con los datos del auto a crear.</param>
    /// <returns><c>201</c> con la ubicación del recurso creado.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCar([FromBody] CreateCarRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = _mapper.Map<CreateCarCommand>(request);
        var carId = await _mediator.Send(command);
        var response = _mapper.Map<CreateCarResponseDto>(carId);

        return CreatedAtAction(nameof(GetCarById), new { id = response.CarId }, response);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// ✏️ Actualiza los datos de un auto existente.
    /// </summary>
    /// <param name="id">Identificador del auto a actualizar.</param>
    /// <param name="request">DTO con los nuevos datos del auto.</param>
    /// <returns><c>204</c> si se actualizó con éxito, o <c>400</c> si hay un conflicto de ID.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent  /* is Ok */  )]
    [ProducesResponseType(StatusCodes.Status400BadRequest /* error */  )]
    [ProducesResponseType(StatusCodes.Status409Conflict   /* error */  )]
    public async Task<IActionResult> UpdateCar(Guid id, [FromBody] UpdateCarDto request)
    {
        if (id != request.Id)
            return BadRequest("El ID del parámetro no coincide con el del cuerpo.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateCarCommand(request.Id, request.Model, request.Type, request.Version);

        await _mediator.Send(command);
        return NoContent();
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 🗑️ Elimina un auto por su <c>Id</c>.
    /// </summary>
    /// <param name="id">Identificador del auto a eliminar.</param>
    /// <returns><c>204</c> si se eliminó correctamente.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent /* is Ok */ )]  
    [ProducesResponseType(StatusCodes.Status404NotFound  /* error */ )]   
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        await _mediator.Send(new DeleteCarCommand(id));
        return NoContent();
    }
}
