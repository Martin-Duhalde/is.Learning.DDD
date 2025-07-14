/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.UseCases.Statistics.GetDailyUsage;
using CarRental.UseCases.Statistics.GetDashboard;
using CarRental.UseCases.Statistics.GetTopCarsByBrandModel;
using CarRental.UseCases.Statistics.GetTopCarTypes;

using MediatR;

using Microsoft.AspNetCore.Authorization;

namespace CarRental.API.Controllers;

/// <summary>
/// 📊 Controller for retrieving various statistical data and reports related to car rentals.
/// </summary>
/// <remarks>
/// Provides endpoints accessible only by Admin users to obtain metrics such as:
/// - Dashboard summary with overall statistics
/// - Top rented car types in a date range
/// - Top cars ranked by brand and model
/// - Daily usage stats including cancellations, rentals, and unused cars
/// 
/// These statistics help in business analysis and decision-making.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class StatisticsController(ILogger<StatisticsController> logger, IMediator mediator) : ControllerBase
{
    private readonly ILogger<StatisticsController>  /**/ _logger = logger;
    private readonly IMediator                      /**/ _mediator = mediator;

    /// <summary>
    /// 📊 Get dashboard summary with daily stats.
    /// </summary>
    /// <remarks>
    /// Returns a snapshot of key metrics to display on the admin dashboard, such as total rentals,
    /// active customers, most used car types, and overall usage trends. Useful for a quick operational overview.
    /// </remarks>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardData()
    {
        var data = await _mediator.Send(new GetDashboardDataQuery());
        return Ok(data);
    }

    /// <summary>
    /// 🏆 Get top 3 rented car types between dates.
    /// </summary>
    /// <remarks>
    /// Retrieves the top 3 most rented car types (e.g., SUV, Sedan) within the specified date range.
    /// Helps identify which car categories are in higher demand.
    /// </remarks>
    [HttpGet("top-car-types")]
    public async Task<IActionResult> GetTopCarTypes([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await _mediator.Send(new GetTopCarTypesQuery(from, to));
        return Ok(result);
    }

    /// <summary>
    /// 🏆 Get top cars ranked by brand, model, and type.
    /// </summary>
    /// <remarks>
    /// Provides statistics on the most rented cars, grouped and ranked by brand, model, and type.
    /// Useful for analyzing customer preferences and optimizing the fleet.
    /// </remarks>
    [HttpGet("top-cars-by-brand-model")]
    public async Task<IActionResult> GetTopCarsByBrandModel([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await _mediator.Send(new GetTopCarsByBrandModelQuery(from, to));
        return Ok(result);
    }

    /// <summary>
    /// 📈 Get daily usage stats: cancellations, rentals, unused cars over the last 7 days.
    /// </summary>
    /// <remarks>
    /// Returns a breakdown of daily activity for the last 7 days, including:
    /// - Number of rentals made
    /// - Number of cancellations
    /// - Number of unused (available but not rented) cars
    /// Useful for monitoring operational performance and demand fluctuations over time.
    /// </remarks>
    [HttpGet("daily-usage")]
    public async Task<IActionResult> GetDailyUsage()
    {
        var result = await _mediator.Send(new GetDailyUsageQuery());
        return Ok(result);
    }
}
