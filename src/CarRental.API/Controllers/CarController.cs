using CarRental.UseCases.Cars.Create;
using CarRental.UseCases.Cars.Delete;
using CarRental.UseCases.Cars.GetAll;
using CarRental.UseCases.Cars.GetById;
using CarRental.UseCases.Cars.Update;

using MediatR;

namespace CarRental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CarController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/car
        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            var query = new ListAllCarsQuery();
            var cars = await _mediator.Send(query);
            return Ok(cars);
        }

        // GET: api/car/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCarById(Guid id)
        {
            var query = new GetCarByIdQuery(id);
            var car = await _mediator.Send(query);
            if (car == null)
                return NotFound();

            return Ok(car);
        }

        // POST: api/car
        [HttpPost]
        public async Task<IActionResult> CreateCar(CreateCarCommand command)
        {
            var carId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCarById), new { id = carId }, null);
        }

        // PUT: api/car/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCar(Guid id, UpdateCarCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);

            await _mediator.Send(command);

            // Si no se lanza excepción, asumimos éxito
            return NoContent();

            //if (!result)
            //    return NotFound();

            //return NoContent();
        }

        // DELETE: api/car/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCar(Guid id)
        {
            {
                await _mediator.Send(new DeleteCarCommand(id));
                return NoContent();
                //var result = await _mediator.Send(new DeleteCarCommand(id));
                //if (!result)
                //    return NotFound();

                //return NoContent();
            }
        }
    }
}
