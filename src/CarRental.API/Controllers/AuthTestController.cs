/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Infrastructure.Auth;
using CarRental.UseCases.Common.Dtos;

using Microsoft.AspNetCore.Authorization;

namespace CarRental.API.Controllers;

/// <summary>
/// 🧪 Auth test endpoints for verifying public and role-based access.
/// </summary>
/// <remarks>
/// This controller provides utility endpoints to test authentication and authorization behavior.
/// It includes:
/// <list type="bullet">
/// <item><description><c>/public</c> — Accessible by anyone.</description></item>
/// <item><description><c>/user</c> — Requires authentication with the "User" role.</description></item>
/// <item><description><c>/admin</c> — Requires authentication with the "Admin" role.</description></item>
/// </list>
/// These endpoints are useful for debugging access control logic during development or testing environments.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class AuthTestController : ControllerBase
{
    /// <summary>
    /// Public endpoint that does not require authentication.
    /// </summary>
    /// <returns>Returns a message confirming public access.</returns>
    /// <response code="200">Public access granted</response>
    /// <remarks>
    /// This endpoint is open to all users.  
    /// It does not require a JWT token or authentication.
    /// </remarks>
    [HttpGet("public")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Public() => Ok("✅ Acceso público: no requiere token.");

    /// <summary>
    /// Endpoint for authenticated users with the "User" role.
    /// </summary>
    /// <returns>Returns a message confirming access with role 'User'.</returns>
    /// <response code="200">Authorized</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    /// <response code="403">Forbidden - user lacks required role</response>
    /// <remarks>
    /// Requires authentication with a valid JWT token.  
    /// Accessible only to users with the <c>User</c> role.
    /// </remarks>
    [Authorize(Roles = RoleNames.User)]
    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
    public IActionResult UserOnly() => Ok("🔐 Acceso con JWT válido (rol 'User').");

    /// <summary>
    /// Endpoint restricted to users with the "Admin" role.
    /// </summary>
    /// <returns>Returns a message confirming access with role 'Admin'.</returns>
    /// <response code="200">Authorized</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    /// <response code="403">Forbidden - user lacks required role</response>
    /// <remarks>
    /// Requires authentication with a valid JWT token.  
    /// Accessible only to users with the <c>Admin</c> role.
    /// </remarks>
    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
    public IActionResult AdminOnly() => Ok("🛡️ Acceso exclusivo para Admin.");
}
