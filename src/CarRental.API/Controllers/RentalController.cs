/// MIT License © 2025 Martín Duhalde + ChatGPT

using AutoMapper;

using CarRental.UseCases.Common.Dtos;
using CarRental.UseCases.Notifications.SendEmail;
using CarRental.UseCases.Rentals.Cancel;
using CarRental.UseCases.Rentals.CheckAvailability;
using CarRental.UseCases.Rentals.Create;
using CarRental.UseCases.Rentals.Dtos;
using CarRental.UseCases.Rentals.Modify;
using CarRental.UseCases.Statistics.GetTopCarTypes;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Authorization;

namespace CarRental.API.Controllers;

/// <summary>
/// 🚗 Controller for managing car rentals including availability, creation, modification, cancellation, and statistics.
/// </summary>
/// <remarks>
/// This controller exposes endpoints to:
/// <list type="bullet">
/// <item><description>Check car availability within a date range filtered by type and model.</description></item>
/// <item><description>Create new rental reservations.</description></item>
/// <item><description>Modify existing rental details.</description></item>
/// <item><description>Cancel rentals logically (mark as cancelled, without deleting records).</description></item>
/// <item><description>Resend confirmation emails for rental bookings.</description></item>
/// <item><description>Retrieve statistics related to top rented car types.</description></item>
/// </list>
/// Authorization is required for most endpoints, with some restricted to admin roles.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class RentalController(ILogger<RentalController> logger, IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly ILogger<RentalController>  /**/ _logger    /**/ = logger;
    private readonly IMediator                  /**/ _mediator  /**/ = mediator;
    private readonly IMapper                    /**/ _mapper    /**/ = mapper;
    /// <summary>
    /// 🚗 Get available cars based on filters.
    /// </summary>
    /// <remarks>
    /// Returns a list of available cars filtered by model, type, and rental period.
    /// Availability is calculated by checking for active reservations that overlap with the requested date range.
    /// </remarks>
    [HttpGet("availability")]
    [ProducesResponseType(typeof(List<CarAvailabilityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckAvailability(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string type,
        [FromQuery] string model)
    {
        var result = await _mediator.Send(new CheckAvailabilityQuery(startDate, endDate, type, model));
        return Ok(result);
    }

    /// <summary>
    /// 📅 Register a new rental.
    /// </summary>
    /// <remarks>
    /// Creates a new reservation for a car. The car must be available during the requested date range,
    /// and the customer must exist. After the rental is created, a confirmation email is sent automatically.
    /// </remarks>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRental([FromBody] CreateRentalRequestDto dto) //CreateRentalCommand command)
    {
        // Manual validation using FluentValidation
        //var validator = HttpContext.RequestServices.GetRequiredService<IValidator<CreateRentalRequestDto>>();
        //var validationResult = await validator.ValidateAsync(dto);
        //if (!validationResult.IsValid)
        //{
        //    // Convert errors to dictionary: property -> error messages
        //    var errors = validationResult.Errors
        //        .GroupBy(e => e.PropertyName)
        //        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        //    return BadRequest(errors);
        //}

        /// TODO: ya tenemos capa de validadores, hay que depurarla

        var command = _mapper.Map<CreateRentalCommand>(dto);                            /// 🧭 Step 1: Map the incoming DTO (from the HTTP request body) to a domain-level command

        var rentalId = await _mediator.Send(command);                                   /// 🚀 Step 2: Send the command to the corresponding MediatR handler, which creates the rental and returns the new rental ID

        var response = _mapper.Map<CreateRentalResponseDto>(rentalId);                  /// 🔁 Step 3: Map the rental ID (GUID) to a response DTO that matches the API contract (e.g., { rentalId: ... })

        await _mediator.Send(new SendReservationConfirmationEmailCommand(rentalId));    /// 📬 Step 4: Trigger a side-effect by sending a separate MediatR command to send a confirmation email

        return Ok(response);                                                            /// ✅ Step 5: Return a 200 OK response with the formatted response DTO (containing the rental ID)
    }

    /// <summary>
    /// ✏️ Modify an existing rental.
    /// </summary>
    /// <remarks>
    /// Updates the rental details, such as dates or assigned car.
    /// The rental ID in the URL must match the one in the request body.
    /// Only active rentals can be modified.
    /// </remarks>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ModifyRental(Guid id, ModifyRentalCommand command)
    {
        if (id != command.RentalId)
            return BadRequest(new ErrorResponseDto { Error = "ID mismatch", Details = "RentalId in body must match URL." });

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// ❌ Cancel an existing rental.
    /// </summary>
    /// <remarks>
    /// This action performs a logical cancellation by updating the rental status to <c>Cancelled</c>.
    /// The reservation is not deleted from the database. Once cancelled, the car becomes available again
    /// for the selected rental period.
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelRental(Guid id)
    {
        await _mediator.Send(new CancelRentalCommand(id));
        return NoContent();
    }

    /// <summary>
    /// 📬 Send a confirmation email again.
    /// </summary>
    /// <remarks>
    /// Triggers a resend of the reservation confirmation email for a specific rental.
    /// Only admins are allowed to use this action.
    /// Useful in case the customer did not receive the original confirmation.
    /// </remarks>
    [HttpPost("{id:guid}/resend-confirmation")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendConfirmation(Guid id)
    {
        await _mediator.Send(new SendReservationConfirmationEmailCommand(id));
        return Ok(new { Message = "Confirmation email resent." });
    }

    /// <summary>
    /// 📊 Get statistics of top rented car types.
    /// </summary>
    /// <remarks>
    /// Returns statistics about the most frequently rented car types within a given date range.
    /// Only accessible to admins. Useful for business insights and demand analysis.
    /// </remarks>
    [HttpGet("statistics/top-car-types")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopRentedCarTypes([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await _mediator.Send(new GetTopCarTypesQuery(from, to));
        return Ok(result);
    }
}
