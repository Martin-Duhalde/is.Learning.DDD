/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.UseCases.Common.Dtos;
using CarRental.UseCases.Rentals.Dtos;
using CarRental.UseCases.Services.GetUpcoming;

using MediatR;

using Microsoft.AspNetCore.Authorization;

namespace CarRental.API.Controllers;

/// <summary>
/// 🛠️ Controller to manage scheduled vehicle services.
/// </summary>
/// <remarks>
/// Provides endpoints to retrieve cars with upcoming scheduled services within specific date ranges.
/// This helps in planning maintenance by showing which cars require service soon.
/// Authorization with Admin role is required to access these endpoints.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ServiceController(ILogger<ServiceController> logger, IMediator mediator) : ControllerBase
{
    private readonly ILogger<ServiceController>     /**/ _logger = logger;
    private readonly IMediator                      /**/ _mediator = mediator;


    /// <summary>
    /// 📅 Get list of cars with scheduled services in the next two weeks.
    /// </summary>
    /// <remarks>
    /// Returns a list of vehicles that have scheduled maintenance or service appointments within the next 14 days.
    /// Useful for maintenance planning, resource allocation, and fleet availability forecasting.
    /// </remarks>
    [HttpGet("upcoming-next-two-weeks")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<UpcomingServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUpcomingServicesNextTwoWeeks()
    {
        var from    /**/ = DateTime.UtcNow.Date;
        var to      /**/ = from.AddDays(14);

        var query   /**/ = new GetUpcomingServicesQuery(from, to);
        var result  /**/ = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// 📅 Get list of cars with scheduled services in a date range (e.g., next two weeks).
    /// </summary>
    /// <remarks>
    /// Retrieves a list of scheduled vehicle services within a custom date range.
    /// Allows admins to monitor and plan upcoming maintenance tasks. 
    /// The <c>from</c> date must not be later than the <c>to</c> date; otherwise, a 400 Bad Request is returned.
    /// </remarks>
    [HttpGet("upcoming")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<UpcomingServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUpcomingServices(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        if (from > to)
        {
            return BadRequest(new ErrorResponseDto
            {
                Error = "Invalid date range",
                Details = "'from' date must be earlier or equal to 'to' date."
            });
        }

        var result = await _mediator.Send(new GetUpcomingServicesQuery(from, to));
        return Ok(result);
    }
}
