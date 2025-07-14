/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Interfaces;
using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRental.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager    /**/ <ApplicationUser>  /**/ _userManager;
    private readonly SignInManager  /**/ <ApplicationUser>  /**/ _signInManager;
    private readonly ILogger        /**/ <AuthService>      /**/ _logger;
    private readonly IConfiguration                         /**/ _config;
    private readonly ICustomerRepository                    /**/ _customerRepository;

    public AuthService(
        UserManager         /**/ <ApplicationUser>  /**/ userManager,
        SignInManager       /**/ <ApplicationUser>  /**/ signInManager,
        ILogger             /**/ <AuthService>      /**/ logger,
        IConfiguration      /**/ config,
        ICustomerRepository /**/ customerRepository
        )
    {
        _userManager        /**/ = userManager;
        _signInManager      /**/ = signInManager;
        _logger             /**/ = logger;
        _config             /**/ = config;
        _customerRepository /**/ = customerRepository;
    }
    public async Task<string> RegisterAsync(string fullName, string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Error creating user: {errors}", errors);
            throw new ApplicationException($"Registration failed: {errors}");
        }

        /// 🔐 Asignar rol por defecto: User
        ///    Más adelante se puede escalar permitiendo que el administrador 
        ///    administre y cambie los roles de los usuarios.
        await _userManager.AddToRoleAsync(user, RoleNames.User);

        /// Asigno el Cliente
        var customer = new Customer
        {
            Id          /**/ = Guid.NewGuid(),
            FullName    /**/ = fullName,
            Address     /**/ = string.Empty,
            UserId      /**/ = user.Id
        };

        await _customerRepository.AddAsync(customer);

        return user.Id;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new InvalidCredentialsException();

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
            throw new InvalidCredentialsException();

        var roles = await _userManager.GetRolesAsync(user); /// Retorna una lista de strings

        // 🔐 Generar Token JWT
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        /// 🔐 Agregar los roles como claims
        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var secretKey = Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpirationMinutes"])),
            claims: authClaims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

}