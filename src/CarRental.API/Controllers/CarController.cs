using CarRental.UseCases.Cars.Create;
using CarRental.UseCases.Cars.Delete;
using CarRental.UseCases.Cars.GetAll;
using CarRental.UseCases.Cars.GetById;
using CarRental.UseCases.Cars.Update;

using MediatR;

namespace CarRental.API.Controllers;

/// <summary>
/// 🎮 Controlador HTTP para operaciones CRUD sobre <c>Car</c>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CarController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor que recibe una instancia de <see cref="IMediator"/>.
    /// </summary>
    /// <param name="mediator">Instancia de MediatR para enviar comandos y queries.</param>
    public CarController(IMediator mediator)
    {
        _mediator = mediator;
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
    /// <param name="command">Comando con los datos del auto a crear.</param>
    /// <returns><c>201</c> con la ubicación del recurso creado.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCar([FromBody] CreateCarCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var carId = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetCarById), new { id = carId }, new CreateCarResponseDto(carId));
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// ✏️ Actualiza los datos de un auto existente.
    /// </summary>
    /// <param name="id">Identificador del auto a actualizar.</param>
    /// <param name="command">Comando con los nuevos datos del auto.</param>
    /// <returns><c>204</c> si se actualizó con éxito, o <c>400</c> si hay un conflicto de ID.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCar(Guid id, [FromBody] UpdateCarCommand command)
    {
        if (id != command.Id)
            return BadRequest("El ID del parámetro no coincide con el del cuerpo.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        await _mediator.Send(new DeleteCarCommand(id));
        return NoContent();
    }
}

//using CarRental.UseCases.Cars.Create;
//using CarRental.UseCases.Cars.Delete;
//using CarRental.UseCases.Cars.GetAll;
//using CarRental.UseCases.Cars.GetById;
//using CarRental.UseCases.Cars.Update;

//using MediatR;

//namespace CarRental.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class CarController : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public CarController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        // GET: api/car
//        [HttpGet]
//        public async Task<IActionResult> GetAllCars()
//        {
//            var query = new ListAllCarsQuery();
//            var cars = await _mediator.Send(query);
//            return Ok(cars);
//        }

//        // GET: api/car/{id}
//        [HttpGet("{id:guid}")]
//        public async Task<IActionResult> GetCarById(Guid id)
//        {
//            var query = new GetCarByIdQuery(id);
//            var car = await _mediator.Send(query);
//            if (car == null)
//                return NotFound();

//            return Ok(car);
//        }

//        // POST: api/car
//        [HttpPost]
//        public async Task<IActionResult> CreateCar(CreateCarCommand command)
//        {
//            var carId = await _mediator.Send(command);
//            return CreatedAtAction(nameof(GetCarById), new { id = carId }, null);
//        }

//        // PUT: api/car/{id}
//        [HttpPut("{id:guid}")]
//        public async Task<IActionResult> UpdateCar(Guid id, UpdateCarCommand command)
//        {
//            if (id != command.Id)
//                return BadRequest("ID mismatch");

//            var result = await _mediator.Send(command);

//            await _mediator.Send(command);

//            // Si no se lanza excepción, asumimos éxito
//            return NoContent();

//            //if (!result)
//            //    return NotFound();

//            //return NoContent();
//        }

//        // DELETE: api/car/{id}
//        [HttpDelete("{id:guid}")]
//        public async Task<IActionResult> DeleteCar(Guid id)
//        {
//            {
//                await _mediator.Send(new DeleteCarCommand(id));
//                return NoContent();
//                //var result = await _mediator.Send(new DeleteCarCommand(id));
//                //if (!result)
//                //    return NotFound();

//                //return NoContent();
//            }
//        }
//    }
//}
