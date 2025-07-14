/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Interfaces;
using CarRental.UseCases.Auth.Dtos;
using CarRental.UseCases.Common.Dtos;

namespace CarRental.API.Controllers;

/// <summary>
/// 🔐 Handles user authentication processes such as registration and login.
/// </summary>
/// <remarks>
/// This controller provides endpoints for user account creation and JWT-based login.  
/// It is publicly accessible and does not require prior authentication.  
/// Upon successful login, a JWT token is returned to be used in subsequent authorized requests.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> log) : ControllerBase
{
    private readonly IAuthService               /**/ _authService       /**/  = authService;
    private readonly ILogger<AuthController>    /**/ _logger            /**/ = log;

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <remarks>
    /// Creates a new user account using full name, email, and password.  
    /// The email must be unique in the system.  
    /// On success, returns the ID of the newly registered user.
    /// </remarks>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Invalid input or user already exists</response>
    /// <response code="500">Unexpected server error</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var userId = await _authService.RegisterAsync(dto.FullName, dto.Email, dto.Password);
        return Ok(new { userId });
    }

    /// <summary>
    /// Logs in a user and returns a JWT token.
    /// </summary>
    /// <remarks>
    /// Authenticates a user with email and password.  
    /// If successful, returns a JWT token to be used for accessing protected endpoints.  
    /// The token contains claims such as user ID and roles.
    /// </remarks>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    /// <response code="500">Unexpected server error</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto.Email, dto.Password);
        return Ok(new LoginResultDto
        {
            Token = token,
            Email = dto.Email,
            UserId = "" /// Envio null, no es necesario enviar el ID, con el JWT alcanza para el login
        });
    }
}
